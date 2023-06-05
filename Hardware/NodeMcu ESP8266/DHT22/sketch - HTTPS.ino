#include "DHT.h"
#include <ESP8266WiFi.h>
#include <ESP8266HTTPClient.h>
#include <WiFiClientSecureBearSSL.h>

#define DHT_PIN 2 // Change data pin if needed
#define DHT_TYPE DHT11

DHT dht(DHT_PIN, DHT_TYPE);

const char* ssid = ""; // Your wlan SSID
const char* password = ""; // Your wlan password
const char* host = ""; // Your server host (ip or hostname)
const int port = 443; // Your server port
const char* deviceName = ""; // Your thermometer name (e.g. the location where it is, like 'kitchen') - you have to escape special chars (e.g. Space [%20])yourself ;)

void setup() {
  Serial.begin(9600);

  Serial.println("Setup");

  dht.begin();
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

  std::unique_ptr<BearSSL::WiFiClientSecure>client(new BearSSL::WiFiClientSecure);
  client->setInsecure();
  HTTPClient https;

  String url = "https://" + String(host) + String(":") + String(port) + "/set?temperature=" + String(temperature) + "&humidity=" + String(humidity) + "&deviceName=" + deviceName;
    
  Serial.println(url);
  Serial.println("[HTTPS] begin...\n");
  if (https.begin(*client, url)) {  // HTTPS
    Serial.println("[HTTPS] GET...\n");
    // start connection and send HTTP header
    int httpCode = https.GET();
    // httpCode will be negative on error
    if (httpCode > 0) {
      // HTTP header has been send and Server response header has been handled
      Serial.printf("[HTTPS] GET... code: %d\n", httpCode);
      // file found at server
      if (httpCode == HTTP_CODE_OK || httpCode == HTTP_CODE_MOVED_PERMANENTLY) {
        String payload = https.getString();
        Serial.println(payload);
      }
    } else {
      Serial.printf("[HTTPS] GET... failed, error: %s\n", https.errorToString(httpCode).c_str());
    }

    https.end();
  } else {
    Serial.printf("[HTTPS] Unable to connect\n");
  }

  Serial.println("Sleep 20 minutes");
  delay(1000 * 60 * 20);
}

