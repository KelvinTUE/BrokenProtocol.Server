Vue.use(VueMaterial.default);

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
        View: "Main",
        Login: {
            Username: "",
            Password: ""
        },
        Device: {
            Name: "Group 1",
            Online: false,
            Sensors: {
                SomeSensor: 15,
                Whatever: "Something"
            }
        },
        menuVisible: false,

        MainContent: "Device"
    },
    methods: {
        login() {

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