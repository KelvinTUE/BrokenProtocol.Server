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
    data: function() {
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

Vue.component("deviceGroup", {
    props: ["group"],
    data: function () {
        return {

        };
    },
    methods: {
        deleteVirtualGroup() {
            api.Simulation.DeleteVirtualGroup("", (data, err) => {
                Vue.nextTick(() => {
                    if (data && !err) {
                        this.showMessage("Virtual Group deleted, wait for server feedback");
                    }
                    else
                        this.showMessage("Failed: " + err);
                });
            });
        }
    },
    template: "#deviceGroup"
})

Vue.component("groupListing", {
    props: ["group", "showOptions"],
    data: function () {
        return {

        };
    },
    methods: {
        addUser() {
            this.$emit("add-user", this.group)
        },
        deleteGroup() {
            api.Management.DeleteGroup(this.group.ObjectID, "", (data, err) => {
                Vue.nextTick(() => {
                    if (!err) {
                        this.$emit("group-changed");
                    }
                });
            });
        },
        deleteUser(id) {
            api.Management.RemoveUserFromGroup(id, this.group.ObjectID, "", (data, err) => {
                Vue.nextTick(() => {
                    if (!err) {
                        this.$emit("group-changed");
                    }
                });
            });
        }
    },
    template: "#groupListing"
});
Vue.component("userTable", {
    props: ["users"],
    data: function () {
        return {
            Error: "",
            OtherError: "",
            NewUser: {
                Username: "",
                Password: "",
                DeviceName: "",
                Error: "",
                InProgress: false
            },
            Selected: []
        }
    },
    methods: {
        selectedUsers(selected) {
            this.Selected = selected;
        },
        updateUsers() {
            this.$emit("users-changed");
        },
        createUser() {
            if (!this.NewUser.Name || !this.NewUser.Password || !this.NewUser.DeviceName)
                return;

            this.NewUser.InProgress = true;
            api.Management.CreateUser({
                Username: this.NewUser.Name,
                Password: this.NewUser.Password,
                DeviceName: this.NewUser.DeviceName,
            }, (data, err) => {
                    Vue.nextTick(() => {
                        if (data) {
                            this.NewUser.InProgress = false;
                            this.NewUser.Username = "";
                            this.NewUser.Password = "";
                            this.NewUser.DeviceName = "";
                            this.NewUser.Error = "";
                        }
                        else
                            this.NewUser.Error = err;
                        this.NewUser.InProgress = false;
                    });
            })
        },
        deleteUser(id) {
            api.Management.DeleteUser(id, "", (data, err) => {
                Vue.nextTick(() => {
                    if (err) {
                        this.OtherError = err;
                    }
                    else
                        this.updateUsers();
                });
            });
        }
    },
    mounted() {
        this.updateUsers();
    },
    template: "#userTable"
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
            IsAdmin: false
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
        Users: [],
        Groups: [],
        NewGroup: {
            Name: "",
            InProgress: false
        },
        NewUser: {
            Username: "",
            Password: "",
            DeviceName: "",
            InProgress: false
        },

        Group: {},

        MainContent: "Dashboard",

        menuVisible: false,

        addUserDialog: {
            Show: false,
            Filter: "",
            Group: {
                Devices: []
            }
        },

        snackPosition: "center",
        snackDuration: 4000,
        snackMessage: "",
        snackActive: false
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
                        this.User.IsAdmin = data.IsAdmin;
                        this.Device.Name = data.DeviceName;
                        this.View = "Main";

                        if (this.User.IsAdmin) {
                            this.updateGroups();
                            this.updateUsers();
                        }
                    });
                    this.startSocket();
                }
            });
        },
        startSocket() {
            api.ManagementSocket({
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
                case "GroupStatus":
                    if (msg.Data) {
                        Vue.nextTick(() => {
                            this.Group = msg.Data;
                        });
                    }
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
        toUsers() {
            this.toView("Users");
        },
        toGroups() {
            this.toView("Groups");
        },
        toGroup() {
            this.toView("Group");
        },

        showAddUserDialog(group) {
            this.addUserDialog.Group = group;
            this.addUserDialog.Show = true;
        },
        showMessage(msg) {
            this.snackMessage = msg;
            this.snackActive = true;
        },

        createVirtualGroup() {
            api.Simulation.CreateVirtualGroup("", (data, err) => {
                Vue.nextTick(() => {
                    if (data && !err) {
                        this.showMessage("Virtual Group created, wait for server feedback");
                    }
                });
            });
        },
        updateUsers() {
            api.Management.Users((data, err) => {
                Vue.nextTick(() => {
                    if (data)
                        this.Users = data;
                    else if (err)
                        this.showMessage(err);
                });
            });
        },
        createUser() {
            this.NewUser.InProgress = true;
            api.Management.CreateUser({
                Username: this.NewUser.Username,
                Password: this.NewUser.Password,
                Devicename: this.NewUser.DeviceName
            },(data, err) => {
                Vue.nextTick(() => {
                    if (data) {
                        this.NewUser.Username = "";
                        this.NewUser.Password = "";
                        this.NewUser.DeviceName = "";
                        this.updateUsers();
                    }
                    else
                        this.showMessage(err);
                    this.NewUser.InProgress = false;
                });
            });
        },
        updateGroups() {
            api.Management.Groups((data, err) => {
                this.Groups = data;
            });
        },
        createGroup() {
            if (!this.NewGroup.Name)
                return;

            this.NewGroup.InProgress = true;
            api.Management.CreateGroup({
                Name: this.NewGroup.Name
            }, (data, err) => {
                    Vue.nextTick(() => {
                        if (data) {
                            this.NewGroup.Name = "";
                            this.updateGroups();
                        }
                        else
                            this.showMessage(err);
                        this.NewGroup.InProgress = false;
                    });
            })
        },
        addUserToGroup(userid, groupid) {
            api.Management.AddUserToGroup(userid, groupid, "", (data, err) => {
                Vue.nextTick(() => {
                    if (err) {
                        this.showMessage(err);
                    }
                    else {
                        this.updateGroups();
                        this.addUserDialog.Show = false;
                    }
                });
            });
        },
        isUserInGroup(userid, group) {
            return group.Devices.filter(x => x.UserID == userid).length > 0;
        }
    },
    mounted(){
        
    }
});