Vue.use(VueMaterial.default);


var api = new SyncAPI(SYNC_CONFIG);


Vue.component("deviceBox", {
    props: ["device"],
    data: function(){
        return {

        }
    },
    template: "#deviceBox"
});

Vue.component("terminal", {
    props: ["device", "blacklist"],
    data: function(){
        return {

        }
    },
    methods: {

    },
    template: "#terminal"
});
Vue.component("sensorBox", {
    props: ["device"],
    data: function(){
        return {

        }
    },
    methods: {

    },
    template: "#sensorBox"
});



new Vue({
    el:'#root',
    data: {
        View: "Login",
        Login: {
            Username: "",
            Password: "",
            Exception: ""
        },
        User: {
            Name: "Group 1",
        },
        Device: {
            Name: "Group 1",
            Online: false,
            LastConnection: undefined,
            Logs: [],
            Sensors: {
                SomeSensor: 15,
                Whatever: "Something"
            }
        },
        menuVisible: false,

        MainContent: "Dashboard"
    },
    methods: {
        login() {
            api.Authentication.Login({
                User: this.Login.Username,
                Password: this.Login.Password
            }, (data) => {
                    if (data && data.Token) {
                        api.updateConfig(data.Token, (success, ex) => {
                            if (success && api.Device) {
                                this.initView();
                            }
                            else {
                                Vue.nextTick(() => {
                                    this.Login.Exception = ex;
                                });
                            }
                        });
                    }
            });
        },
        initView() {
            api.User.GetUserData((data) => {
                if (data && data.Name) {
                    Vue.nextTick(() => {
                        this.User.Name = data.Name;
                        this.Device.Name = data.DeviceName;
                        this.View = "Main";
                    });
                    this.startSocket();
                }
            });
        },
        startSocket() {
            api.Management({
                open: (ev) => {
                    console.log("Websocket Open");
                },
                close: (ev) => {
                    console.log("Websocket Close");
                },
                message: (ev) => {
                    console.log("WebSocket:", ev);
                    try {
                        this.handleMessage(JSON.parse(ev.data));
                    }
                    catch (ex) {
                        console.log("Handle Exception:" + ex, ev);
                    }
                },
                error: (ev) => {
                    console.log(ev);
                }
            });
        },
        handleMessage(msg) {
            switch (msg.Type) {
                case "Log":
                    if (msg.Data) {
                        Vue.nextTick(() => {
                            this.Device.Logs.push(msg.Data);
                        });
                    }
                    break;
                case "SensorData":
                    if (msg.Data.Data)
                        Vue.nextTick(() => {
                            this.Device.Sensors = msg.Data.Data;
                        });
                    break;
                case "UserStatus":
                    if (msg.Data) {
                        Vue.nextTick(() => {
                            this.Device.Online = msg.Data.Online;
                            var date = new Date(msg.Data.LastActivity);
                            if (date > new Date(2020, 1, 1))
                                this.Device.LastConnection =
                                    date.getHours() + ":"
                                + ("" + date.getMinutes()).padStart(2, '0') + ":"
                                + ("" + date.getSeconds()).padStart(2, '0') + " "
                                    + date.getFullYear() + "-"
                                    + date.getMonth() + "-"
                                    + date.getDate();
                        });
                    }
                    break;
            }
        },
        toggleMenu () {
            this.menuVisible = !this.menuVisible
        },
        toView(view) {
            this.MainContent = view;
        },
        toDevice() {
            this.toView("Device");
        },
        toDashboard() {
            this.toView("Dashboard");
        },
        toGroup() {
            this.toView("Group");
        }
    },
    mounted(){
        
    }
});