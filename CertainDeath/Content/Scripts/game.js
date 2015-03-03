/// <reference path="phaser.js" />
/// <reference path="view.js" />
var game;
window.onload = function () {
    var elm = document.getElementById("content");
    game = new Phaser.Game(elm.offsetWidth, elm.offsetWidth, Phaser.AUTO, "content", { preload: preload, create: create, update: update, render: render }, false);
}

var shop;

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
    shop = new View.BuildingShop(game, Server, 40, 40, game.world.width - 80, game.world.height - 80);
    var views = [
        shop,
        new View.ButtonScreen(game, 20, 10, 70, 32*2, "objects", "ShopButton", openShop),
        new View.InventoryBar(game, 100, 10, game.width - 20 - 100, 32*2),
        new View.MainGameScreen(game, Server, 30, 20 + 32 * 2, game.width - 60, game.height - 30 - 20 - 32 * 2),
    ];
    View.current = new View.ScreenContainer(views, 0, 0, game.width, game.height);
    View.current.create();
    Server.register(View.current);
    View.current.screens.forEach(function (val) {
        Server.register(val);
    });
    Server.open();

    game.stage.backgroundColor = 0xbbddff;
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

function openShop() {
    if (shop) {
        shop.visible = true;
        console.log("opening shop");
    }
}

Server = (function () {
    var socket = null;
    var listeners = new Array();
    function open() {
        socket = new WebSocket("wss://" + window.location.host + "/api/WebSocket/" + WorldId);
        socket.onmessage = onmessage;
    }

    function register(listener) {
        listeners.push(listener);
    }

    function unregister(listener){
        throw new Error("Not yet implemented.");
    }

    function onmessage(message) {
        var obj = JSON.parse(message.data);
        for (var x = 0; x < listeners.length; ++x) {
            for (var y = 0; y < listeners[x].subscribesTo.length; y++) {
                var subs = listeners[x].subscribesTo[y].split('.');
                var prop = obj;
                var worked = true;
                for (var z = 0; z < subs.length; z++) {
                    if (prop[subs[z]]) {
                        prop = prop[subs[z]];
                    } else {
                        worked = false;
                        break;
                    }
                }
                if (worked) {
                    listeners[x].onmessage(prop, listeners[x].subscribesTo[y]);
                }
            }
        }
    }

    window.onbeforeunload = function () {
        socket.close();
    }

    function close() {
        socket.close();
    }

    function send(msg) {
        socket.send(msg);
    }

    return {
        open: open,
        register: register,
        close: close,
        send: send
    }
})();