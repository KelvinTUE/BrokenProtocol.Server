# **[2IO70] Protocol Documentation**

**By BrokenProtocol (Team 1)**

In this document we briefly cover the currently planned/present communication protocol, associated API endpoints, and functionality that will facilitate communication for devices.

We recommend you fully read it before asking for features to prevent duplicate requests.

The actual API Specification can be found after all other documentation at the bottom.

## Why am I reading this

As explained in the course introduction, the idea is that the device of each group has to communicate with each other to determine "fairness" when it comes to object distribution. This is done to prevent one device from consuming all objects on the belt. Our team is in charge of facilitating this networking. We have however decided to make life easier (for you), we will be providing a centralized authority so that devices do not actually have to negotiate with each other, and instead the server will take care of the fairness. You simply have to listen to what the server tells you, and provide it with basic information.

Note that while we do assume devices are taking objects from the same belt, in practice this won't be the case in the final demo session. 

## Networking/Data Format

The currently planned networking protocol is HTTP(S), at the time of writing it is exclusively available over HTTPs. This should be available for any language or runtime that you may have chosen. On top of this we use JSON as data format. Most modern languages have JSON serialization available in their standard frameworks. If not there must be a library available (e.g. For C/C++ you can use cJson).

Besides HTTP we may also use Websocket for some specific features, however, knowing that this may be harder to get working under certain circumstances, we will try to provide all important functionality through exclusively HTTP(s).

The server will take the form of a standard REST API, you can learn more about how REST APIs work here:

[![](https://img.youtube.com/vi/-MTSQjw5DrM/0.jpg)](https://www.youtube.com/watch?v=-MTSQjw5DrM)

(The first 100 seconds)

---

# **GENERAL OVERVIEW**

All communication with the server is done through individual HTTP requests, meaning that for each respective step in the process you will make a http request to server appropriate REST endpoint.

## Expected Behavior

Below you will see a simple UML activity diagram on how we expect the usage of the service to work from the device's perspective.

![](https://raw.githubusercontent.com/KelvinTUE/BrokenProtocol.Server/master/Images/Flowchart.jpg)
<br />
Thus a minimal implementation of the protocol uses only 3 calls. Besides these endpoints you may have to use GET: /Device/Heartbeat or POST: /Device/SensorData to indicate your device is alive if you do not communicate with the server within 5 seconds.

## Additional Functionality

In addition to the base functionality, we will be providing extra tools to help you out with implementing the protocol and working with your device.

### Web GUI

We will be providing a website where you can follow the activity of your device, such as online status, and other features below.

### Logging

We will offer a way for you to log to our API. These logs will be visible when you are looking at the device tab on the web portal. This will allow you to get live feedback from your device while you are not directly looking at the shell for whatever reason.

### Commands

In addition to logging, we also allow you to send commands from the GUI to your device, this does require you to actually request/listen for those commands and implement them yourself. We provide this functionality for you to use in whatever way you think is useful, or not use at all.

### Simulation

We will be providing an option to enable a fake device group, with emulated devices. This way you can actually test your device working with other devices "connected". This can be controlled through the Web GUI

---

# **USAGE WARNING**

Some of these endpoints may be tempting to call at extremely fast intervals, do note that doing so may cause your requests to be blocked for a certain amount of time. The server is used by more teams than you, while the server should be more than powerful enough to handle all of you, we do not need a DDOS attack. DDOS protection is present, and may trigger as a false positive. We will be logging group activity.

---

# **FAQ**

---

### Why HTTPS? And why not [Insert obscure protocol]?

HTTP is used almost everywhere for communication, it is extremely simple and text based. We considered a custom TCP protocol but this would require more work on the side of devices, and may be more annoying with certain languages. We choose this over MQTT which is used in previous years, because it would be more decentralized, require all devices to act accordingly, and consider devices not doing their job properly. In the current system we have a central authority that can handle device independently. The server also tells you if you screwed up.

### Can I request another feature?

We believe this protocol will provide the basics of whatever machine you want to build. If you read the entire specification and for whatever reason you feel like you need another feature, feel free to post on the Protocol Discussion board in Teams, we occasionally check this and may reply as necessary. Keep in mind that we also have to make a machine just like you and that networking is just additional work we do. So please keep the requests reasonable.

### Feature: Can I place objects back on the belt?

So, we've heard this request a few times, and we even considered it ourselves in our initial machine. However the reason we decided not to use such features was because there will NOT be a physical belt connecting machines. Putting things back on the belt would imply a human picking up the item. So based on this knowledge we decided such a feature would be meaningless, and thus, the networking does not provide it. However, if you feel like you really need this feature, we can consider it but we'll have to discuss with staff how this would impact the fairness algorithm since it doesn't make sense in practice. But in our opinion, every item you take is consumed by your machine and won't go back into the system. <br />
**UPDATE:** After further requests, we'll be adding another endpoint later allowing you to "put back objects". Documentation will be updated when available.

### Can I use HTTP instead of HTTPS?

At the time of writing, HTTP is disabled. If you are writing the HTTP protocol from scratch over TCP for whatever reason, we understand that this may be hard. In this case we may enable HTTP. Using a library is however recommended to avoid issues. SSL Certificates should always be valid as these are provided by Cloudflare.

### I can't get it to work, can you help us?

We will be extensively testing this api, however if after various attempts you cannot get it to work you can try getting in contact with group 1 through Teams, we will occasionally check the protocol discussion channel for activity.

### What is the server running on?

The server is written in C# ASP.Net Core v3 within a custom wrapper (but effectively just ASP.Net Core v3). The server is hosted behind Cloudflare, and will be running on a server provided by a member of the BrokenProtocol team. The server can be upgraded to be quite beefy, but we rather not. At the time of writing its running in a VM with 3 cores and 4 gigs of RAM. Supposedly overkill for the application. Bandwidth is more limited though.

### Can I host the server locally?

Yes, you could. Source code is available and we might provide releases (for Linux and Windows). However, do know we will be providing the actual server, so you might as well use it. The server can run on Windows and Linux without additional dependencies. You may require admin-rights to bind to the right ports. We will not be providing extensive support to host through, but we'll be open to simple questions.

### Can I contribute/change server code?

We'd rather not allow everyone the flood us with pull requests on git. You're better off spending your time on your device. If you want changes, send us a message through Teams or something.

### Do you have samples in [Insert my obscure Language]?

As these are simple HTTP requests, you should find plenty of examples of using HTTP requests on the internet. Every languages should be able to do it, and most languages are shipped with it.

### Is source code available?

Yes, we will be providing the source code for the **Server, Unit Tests** and **C# Client Library.** The latter 2 should get you on your way. While we do not believe the Server code will help you, it will be available.

### Can we see your device code?

No.

### Why not?

It means you wouldn't have to write any actual code. But actually, It would be considered fraud by university.

---

# **API ENDPOINTS**

Here we will show you the endpoints we will provide with a brief description. In general the API will try to be flexible. The main thing that is enforced is Content-Type and authentication. In some circumstances you might have to add a proper Host header if it was not done automatically, but we don't believe this is the case.

The following endpoints will be accessible on a specific domain to be published. But you can assume there will be a domain with https accessible. (eg. https://somedomain.com/whateverEndpoint).

The domain as well as your account will be provided through Teams, and won't be posted publicly.

## Main Endpoints

These are the endpoints mandatory to use to adhere to the protocol.

---

### **POST** /Authentication/Login

*Returns a json object with property "token" containing a string used for authentication for all requests. Generally this is through the "auth" header.*

##### **HEADERS**

---

```
Content-Type: "application/json"
```

##### **REQUEST BODY**

---

```
{   
    "User": "YourGroupUsername",   
    "Password": "YourGroupPassword"
}
```

##### **RESPONSE**

---

```
{   
   "Token": "TokenToUseInAuthHeader"
}
```

---

---

### **GET** /Device/Heartbeat

*Used to inform the server that your device is still alive (Can be substituted by /Device/SensorData)*

Returns nothing

##### **HEADERS**

---

```
auth: "{YourAuthenticationToken}"
```

---

---

### **GET** /Device/CanPickup

*Used to determine if you're allowed to pick up the next detected object.*

Returns true if you are allowed to pick up the next detected object.\
Returns false if you are not allowed to pick up the next detected object.

##### **HEADERS**

---

```
auth: "{YourAuthenticationToken}"
```

---

---

### **POST** /Device/PickedUpObject

*Indicates that your device picked up an object.*

Returns true if this is as expected. \
Returns false if this was not expected (thus violating the protocol).

##### **HEADERS**

---

```
auth: "{YourAuthenticationToken}"
```
---
---

### **POST** /Device/PutBackObject

*Indicates that your device put the object you picked up back on the feeding belt* \
*NOTE: Feeding belts will not be connected in final demo (so your objects will not move to other machines unless humans move them). But it will affect the fairness algorithm.*

Returns true if this is as expected. \
Returns false if this was not expected (thus violating the protocol).

##### **HEADERS**

---

```
auth: "{YourAuthenticationToken}"
```

---

---

### **POST** /Device/DeterminedObject

*Inform the server on the color of your object. This endpoint is not strictly required to conform to a functioning protocol, but is recommend so that we get statistics.*

##### **HEADERS**

---

```
Content-Type: "application/json"
auth: "{YourAuthenticationToken}"
```

##### **REQUEST BODY**

---

```
{   
    "Color": 0                 //0=Black, 1=White
}
```

---

## Optional Endpoints

These are the endpoints that may offer you additional functionality during development.

---

### **POST** /Device/Log

*Used to send logs to your web browser, note that logs are **NOT** stored on the server, and are only received when you have your browser actively logged in.*

##### **HEADERS**

---

```
Content-Type: "application/json"
auth: "{YourAuthenticationToken}"
```

##### **REQUEST BODY**

---

```
{   
    "Tags": ["SomeTag"],
    "Message": "Whatever you want to tell yourself to sleep at night"
}
```

**Tags**\
Used to identify your message, can be used to filter out logs in GUI.\
**Message**\
The message you will see in your GUI

---

---

### **POST** /Device/SensorData

*Used to send sensor data to your web browser, note that sensor data is **NOT** stored on the server, and are only received when you have your browser actively logged in.*

##### **HEADERS**

---

```
Content-Type: "application/json"
auth: "{YourAuthenticationToken}"
```

##### **REQUEST BODY**

---

```
{   
    "Data": {
      "Sensor Name 1": 0.5,
      "Sensor Name 2": "Some text",
    }
}
```

**Data**\
Here you can add sensor data. Each sensor you would like to add has a property on Data as a Key-Value pairing.
