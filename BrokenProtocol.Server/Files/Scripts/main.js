Vue.use(VueMaterial.default);


var api = new SyncAPI(SYNC_CONFIG);

Vue.component('device-chart', {
    props: ["group"],//["labels", "consumed", "white", "black"],
    extends: VueChartJs.Bar,
    data: () => {
        return {
            chartObj: {
                labels: [],
                datasets: [
                    {
                        label: "Consumed",
                        backgroundColor: '#2E7D32',
                        data: []
                    },
                    {
                        label: "Black",
                        backgroundColor: 'black',
                        data: []
                    },
                    {
                        label: "White",
                        backgroundColor: 'white',
                        data: []
                    }
                ]
            },
            chartOptions: {
                scales: {
                    yAxes: [{
                        ticks: {
                            beginAtZero: true
                        }
                    }]
                },
                responsive: true,
                maintainAspectRatio: false,
                animation: false
            }
        };
    },
    mounted() {
        this.updateData();
        this.renderChart(this.chartObj, this.chartOptions);
    },
    methods: {
        updateData() {
            var val = this.group;
            var labels = this.chartObj.labels;
            var consumed = this.chartObj.datasets[0].data;
            var black = this.chartObj.datasets[1].data;
            var white = this.chartObj.datasets[2].data;
            labels.splice(0, labels.length);
            consumed.splice(0, consumed.length);
            black.splice(0, black.length);
            white.splice(0, white.length);
            if (val && val.Devices) {
                for (var i = 0; i < val.Devices.length; i++) {
                    var dev = val.Devices[i];
                    labels.push(dev.Name);
                    consumed.push(dev.TotalCount);
                    if (dev.ObjectCounts.White)
                        white.push(dev.ObjectCounts.White);
                    else
                        white.push(0);
                    if (dev.ObjectCounts.Black)
                        black.push(dev.ObjectCounts.Black);
                    else
                        black.push(0);
                }
            }
        },
        render() {
            //this._data._chart.update()
            this.renderChart(this.chartObj, this.chartOptions);
        }
    },
    watch: {
        group(val) {
            this.updateData();
            this.render();
        }
    }
});

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
    watch: {
        "device.Logs"() {
            this.scrollBottom();
        }
    },
    methods: {
        scrollBottom() {
            Vue.nextTick(() => {
                if (this.$refs.log) {
                    this.$refs.log.scrollTop = this.$refs.log.scrollHeight;
                }
            });
        }
    },
    template: "#terminal"
});
Vue.component("terminal-only", {
    props: ["logs"],
    data: function () {
        return {
        }
    },
    watch: {
        "logs"() {
            this.scrollBottom();
        }
    },
    methods: {
        scrollBottom() {
            Vue.nextTick(() => {
                if (this.$refs.log) {
                    this.$refs.log.scrollTop = this.$refs.log.scrollHeight;
                }
            });
        }
    },
    template: "#terminal-only"
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
        SelectedUserLog: "",
        UserLogs: [

        ],

        MonitorSelectedGroup: "",
        MonitorGroups: [

        ],

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

        Group: undefined,

        MainContent: "Dashboard",

        menuVisible: false,

        addUserDialog: {
            Show: false,
            Filter: "",
            Group: {
                Devices: []
            }
        },
        createVirtualGroupDialog: {
            Show: false,
            Options: {
                AllowOfflinePickup: false,
                SecondsObjects: 3,
                SecondsPickup: 1,
                SecondsDetermine: 2,
                SecondsVariance: 1
            }
        },

        snackPosition: "center",
        snackDuration: 4000,
        snackMessage: "",
        snackActive: false,
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
                    Vue.nextTick(() => {
                        this.Group = msg.Data;
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
                case "UserLog":
                    if (msg.Data) {
                        Vue.nextTick(() => {
                            var user = msg.Data.UserID;
                            var userLog = this.getUserLog(user);
                            var time = new Date();
                            msg.Data.Log.Time = time.getHours() + ":" + ("" + time.getMinutes()).padStart(2, '0') + ":" + ("" + time.getSeconds()).padStart(2, '0');
                            userLog.Logs.push(msg.Data.Log);
                        });
                    }
                    break;
                case "AdminGroupsStatus":
                    if (msg.Data) {
                        Vue.nextTick(() => {

                            for (var i = 0; i < msg.Data.length; i++) {
                                var group = msg.Data[i];
                                var groupIndex = this.getGroupIndex(group.ObjectID);

                                if (groupIndex < 0)
                                    this.MonitorGroups.push(group);
                                else
                                    Vue.set(this.MonitorGroups, groupIndex, group);
                            }

                        });
                    }
                    break;
            }
        },
        getGroupIndex(id) {
            for (var i = 0; i < this.MonitorGroups.length; i++) {
                if (this.MonitorGroups[i].ObjectID == id)
                    return i;
            }
            return -1;
        },
        getUserLog(id) {
            var log = undefined;
            for (var i = 0; i < this.UserLogs.length; i++) {
                if (this.UserLogs[i].ID == id) {
                    log = this.UserLogs[i];
                    break;
                }
            }
            if (!log) {
                var user = this.getUser(id);

                log = {
                    ID: id,
                    Name: user.Name,
                    Logs: []
                };
                this.UserLogs.push(log);
            }
            return log;
        },
        getUser(id) {
            for (var i = 0; i < this.Users.length; i++) {
                if (this.Users[i].ObjectID == id) {
                    return this.Users[i];
                }
            }
            return undefined;
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
        toMonitor() {
            this.toView("Monitor");
        },
        navigate(url) {
            window.location.href = url
        },

        showAddUserDialog(group) {
            this.addUserDialog.Group = group;
            this.addUserDialog.Show = true;
        },
        showMessage(msg) {
            this.snackMessage = msg;
            this.snackActive = true;
        },

        createVirtualGroup(options) {
            api.Simulation.CreateVirtualGroup(options, (data, err) => {
                Vue.nextTick(() => {
                    if (data && !err) {
                        this.createVirtualGroupDialog.Show = false;
                        this.showMessage("Virtual Group created, wait for server feedback");
                    }
                    else {
                        this.showMessage("Virtual Group creation failed: " + err);
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
        },

        resetGroupValues(id) {
            api.Management.ResetGroupValues(id, "", (data, err) => {
                Vue.nextTick(() => {
                    if (data && !err) {
                        this.showMessage("Values for group [" + id + "] reset");
                    }
                    else
                        this.showMessage("Failed: " + err);
                });
            });
        },


        clearUserLogs() {
            this.UserLogs = [];
        }
    },
    mounted(){
        
    }
});