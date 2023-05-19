using Configurations;
using Models.Database;
using MySql.Data.MySqlClient;
using NLog;

namespace Repositories
{
    public class DatabaseRepository
    {
        protected MySqlConnection Connection { get; set; }
        protected MySqlTransaction? Transaction { get; set; } = null;
        private Logger Logger { get; set; } = LogManager.GetCurrentClassLogger();

        public DatabaseRepository(ApplicationConfiguration configuration)
        {
            Connection = new MySqlConnection($"Server={configuration.DatabaseServer};Database={configuration.DatabaseName};User Id={configuration.DatabaseUser};Password={configuration.DatabasePassword};");
        }

        public int ExecuteNonQuery(string commandText, params object[] parameters)
        {
            try
            {
                Connection.Open();
                return GetCommand(commandText, parameters).ExecuteNonQuery();
            }
            catch (Exception ex)
            {
                Logger.Error(ex.Message);
                throw;
            }
            finally
            {
                Connection.Close();
            }
        }

        public Table ExecuteReader(string commandText, params object[] parameters)
        {
            try
            {
                Connection.Open();

                MySqlDataReader reader = GetCommand(commandText, parameters).ExecuteReader();

                Table table = new()
                {
                    DataTable = reader.GetSchemaTable()
                };

                while (reader.Read())
                {
                    Row row = new();

                    for (int i = 0; i < reader.FieldCount; i++)
                    {
                        row.Columns.Add(new(reader.GetName(i), reader.GetValue(i)));
                    }

                    table.Rows.Add(row);
                }

                return table;
            }
            catch (Exception ex)
            {
                Logger.Error(ex.Message);
                throw;
            }
            finally
            {
                Connection.Close();
            }
        }

        public T ExecuteScalar<T>(string commandText, params object[] parameters)
        {
            try
            {
                Connection.Open();
                return (T)GetCommand(commandText, parameters).ExecuteScalar();
            }
            catch (Exception ex)
            {
                Logger.Error(ex.Message);
                throw;
            }
            finally
            {
                Connection.Close();
            }
        }

        public void ExecuteTransaction(List<Query> queries)
        {
            try
            {
                StartTransaction();

                foreach (Query query in queries)
                {
                    GetCommand(query.CommandText, query.Parameters).ExecuteNonQuery();
                }

                CommitTransaction();
            }
            catch (Exception ex)
            {
                Logger.Error(ex.Message);
                RollbackTransaction();
                throw;
            }
            finally
            {
                Connection.Close();
            }
        }

        public void ExecuteTransaction(string commandText, params object[] parameters)
        {
            ExecuteTransaction(new List<Query>
            {
                new Query(commandText, parameters)
            });
        }

        public bool TestConnection(out string error)
        {
            try
            {
                Connection.Open();

                error = "";
                return true;
            }
            catch (Exception ex)
            {
                Logger.Error(ex.Message);
                error = ex.Message;
                return false;
            }
            finally
            {
                Connection.Close();
            }
        }

        /// <summary>
        /// Rolls back the current transaction and closes the connection
        /// </summary>
        private void RollbackTransaction()
        {
            try
            {
                Transaction?.Rollback();
            }
            finally
            {
                Connection.Close();
                Transaction?.Dispose();
                Transaction = null;
            }

        }

        /// <summary>
        /// Committs the current transaction and closes the connection
        /// </summary>
        private void CommitTransaction()
        {
            try
            {
                Transaction?.Commit();
            }
            finally
            {
                Connection.Close();
                Transaction?.Dispose();
                Transaction = null;
            }
        }

        /// <summary>
        /// Starts the current transaction and opens a connection
        /// </summary>
        private void StartTransaction()
        {
            try
            {
                if (Connection.State != System.Data.ConnectionState.Open)
                {
                    Connection.Open();
                }

                Transaction = Connection.BeginTransaction();
            }
            catch (Exception ex)
            {
                Logger.Error(ex.Message);

                Transaction?.Dispose();
                Transaction = null;
                throw;
            }

        }

        /// <summary>
        /// Creates a new command with parameters and prepares it
        /// </summary>
        /// <param name="commandText">The command to be executed</param>
        /// <param name="parameters">The parameters of the command</param>
        /// <returns></returns>
        private MySqlCommand GetCommand(string commandText, params object[] parameters)
        {
            MySqlCommand command = Connection.CreateCommand();
            Logger.Debug($"{commandText} - {string.Join(',', parameters)}");

            command.CommandText = commandText;
            command.Connection = Connection;
            if (Transaction != null) command.Transaction = Transaction;

            for (int i = 0; i < parameters.Length; i++) { command.Parameters.AddWithValue($"@{i}", parameters[i]); }

            command.Prepare();

            return command;
        }

        /// <summary>
        /// Disposes the transaction and the connection
        /// </summary>
        public void Dispose()
        {
            Transaction?.Dispose();

            Connection?.Close();
            Connection?.Dispose();
        }

    }
}
