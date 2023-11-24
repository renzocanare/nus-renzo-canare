<h1 align="center">📄 EE4204 - Computer Networks 📄</h1>

### 🚀 Exercise 1 - TCP/UDP Short Message

#### Tasks:
- Develop a socket program that uses TCP and UDP for transmission of short messages.

### :flashlight: Exercise 2 - TCP Large Message

#### Tasks:
- Develop a TCP-based client-server socket program for transferring a large message: 
    - The entire message is sent by the client as a single data-unit.

### ✨ Exercise 3 - TCP Large Message Split Data Unit

#### Tasks:
- Develop a TCP-based client-server socket program for transferring a large message: 
    - The message is split into short data-units which are sent one by one without waiting for 
any acknowledgement between transmissions of two successive data-units.

### ☀️ Exercise 4 - TCP "Jumping Window" Protocol

#### Tasks:
- Develop a TCP-based client-server socket program for transferring a large message:
    - Use a hypothetical jumping window protocol where *n* data-units are sent, waits for an acknowledgement, and then sends the next *n* data-units.
    - Repeat until the entire file is sent and the acknowledgement for the last batch is received.