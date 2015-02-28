/// <reference path="phaser.js" />
/// <reference path="view.js" />
var game;
window.onload = function () {
    var elm = document.getElementById("content");
    game = new Phaser.Game(elm.offsetWidth, elm.offsetWidth, Phaser.AUTO, "content", { preload: preload, create: create, update: update, render: render });
}

var buildings;
var resources;
var monsters;
var grass;

// The screen that is being rendered by the game
// View.current

// The server object used to pull data from the server
var server;

function preload () {
    // download all sprites
    game.load.atlas("objects", "/Content/Images/spritesheet2.png", "/Content/Images/spritesheet2.json");
    //server = new Server("", onerror, onclose);
}

function create () {
    // add all objects to game to be displayed
    //grass = new game.add.sprite(game.world.centerX, 500, 'grass', 'objects');
    //grass = game.add.tileSprite(0, 0, game.world.width, game.world.height, "objects", "grass");
    //View.current = new View.MainGameScreen(game, Server, 300, 30, game.width - 60, game.height - 60);
    var views = [
        new View.InventoryBar(game, 10, 10, game.width - 20, 32*3),
        new View.MainGameScreen(game, 30, 20 + 32 * 3, game.width - 60, game.height - 30 - 20 - 32 * 3),
    ];
    View.current = new View.ScreenContainer(views, 0, 0, game.width, game.height);
    View.current.create();
    Server.register(View.current);
    View.current.screens.forEach(function (val) {
        Server.register(val);
    });
    Server.open();
    // Uncomment later
    //game.stage.backgroundColor = 0xbbddff;
}

function update () {
    // called every frame
    View.current.update();
}

function render () {
    // I don't know what this is for
    // Docs say it's not used often, so...
    View.current.render();
}

Server = (function () {
    var socket = null;
    var listeners = new Array();
    function open() {
        socket = new WebSocket("wss://" + window.location.host + "/api/WebSocket/10");
        socket.onmessage = onmessage;
    }

    function register(listener) {
        listeners.push(listener);
    }

    function unregister(listener){
        throw new Error("Not yet implemented.");
    }

    function onmessage(message){
        //this.messages.push(JSON.parse(message.data));
        console.log(message.data);
        var obj = JSON.parse(message.data);
        for (var x = 0; x < listeners.length; ++x) {
            if (obj[listeners[x].subscribesTo]) {
                listeners[x].onmessage(obj[listeners[x].subscribesTo]);
            }
        }
    }

    window.onbeforeunload = function () {
        socket.close();
    }

    function close() {

    }

    return {
        open: open,
        register: register,
        close: close
    }
})();