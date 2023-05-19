#include "DHT.h"
#include <ESP8266WiFi.h>
#include <WiFiClient.h>

#define DHT_PIN 2 // change pin if needed
#define DHT_TYPE DHT11

DHT dht(DHT_PIN, DHT_TYPE);

const char* ssid = "<Your WLAN Name>";
const char* password = "<Your Wlan password>";
const char* host = "<Your server ip or host name (without http://)>";
const int port = 80; // Your server port
const char* deviceName = ""; // your thermometer name (e.g. the location where it is, like 'kitchen')

void setup() {
  Serial.begin(9600);

  Serial.println("Setup");

  dht.begin();

  WiFi.disconnect();
  WiFi.mode(WIFI_STA);
  WiFi.begin(ssid, password);
  
  Serial.println("Connecting Wifi");
  while (WiFi.status() != WL_CONNECTED) {
    delay(500);
    Serial.print(".");
  }

  Serial.println("");
  Serial.print("Connected to ");
  Serial.println(ssid);
  Serial.print("IP: ");
  Serial.println(WiFi.localIP());

  delay(1000);
  Serial.println("Started");
}

void loop() {
  Serial.println("Refresh values");
  float temperature = dht.readTemperature();
  float humidity = dht.readHumidity();

  Serial.print("connecting to ");
  Serial.print(host);
  Serial.print(':');
  Serial.println(port);

  WiFiClient client;
  if (!client.connect(host, port)) {
    Serial.println("connection failed");
    delay(500);
    return;
  }

  Serial.print("connected");

  String url = "/set?temperature="+ String(temperature) + "&humidity=" + String(humidity);
  Serial.print("Requesting URL: ");
  Serial.println(url);
  
  client.print(String("GET ") + url + " HTTP/1.1\r\n" +
               "Host: " + host + "\r\n" + 
               "Connection: close\r\n\r\n");

  delay(10);
  
  Serial.println("Response:");
  while(client.available()){
    String line = client.readStringUntil('\r');
    Serial.print(line);
  }
  
  Serial.println();
  Serial.println("closing connection");

  Serial.println("Sleep 30 minutes");
  delay(1000 * 60 * 30);
}

