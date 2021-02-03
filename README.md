## 🔍 Gadget, modern solution to not so modern problem
![](https://i.imgur.com/Twn5yg8.png)
---
### Functionality
* Remote Windows services management, ability to turn a service on or off
* Notifications, webhooks and websockets, more to come
---
### Bulding blocks
- **Gadget.Inspector** is an agent that runs on each of your Windows machines you'd wish to monitor

- **Gadget.Server** is a control plane like service, it commands Inspectors to invoke requested actions, like turn off service **A**, restart service **B**

- **Gadget.Notifications** is a notifications service, whenever service of your interest change its state it notifies all configured parties

- **Gadget.Messaging** contains contracts shared between services 
---
### Caveats 
- **Gadget.Inspector** needs to be run as a user with rights to modify installed Windows services (basically administrator account or the one you've used to install your services)
---
#### Architecture behind Gadget
![](https://i.imgur.com/dEuEPRc.png)

#### Dependency Diagram
![](https://i.imgur.com/kTpGly9.png)
