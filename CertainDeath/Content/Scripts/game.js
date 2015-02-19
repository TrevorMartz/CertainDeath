/// <reference path="phaser.js" />
var game = new Phaser.Game(600, 600, Phaser.AUTO, "content", { preload: preload, create: create, update: update, render: render});
var CD = {};

/**
 * Objects in js are weird. I'm never
 * going to use this object, nor will I
 * inherit from it, but it is the basic
 * guideline for what I will need to
 * include in a screen object
 **/
function Screen(update, render) {
    this.update = update;
    this.render = render;
    return this;
}

/// list of screens
// loading
// game
// various menus
// 

CD.LoadingScreen = function () {
    this.text = null;
    this.graphics = null;
};

CD.LoadingScreen.prototype = {
    create: function () {
        this.graphics = game.add.graphics(0, 0);
        this.graphics.beginFill(0xffffff);
        this.graphics.drawRect(0, 0, game.width, game.height);
        this.text = game.add.text(game.world.centerX, game.world.centerY, "loading...");
    },

    update: function () {

    },

    click: function (evt) {

    },

    destroy: function () {
        this.text.destroy();
        this.graphics.destroy();
    }
}

CD.MainGameScreen = function (server) {
    this.server = server;
    var trees = new Array();

}

var buildings;
var resources;
var monsters;
var grass;

// The screen that is being rendered by the game
CD.screen = {};

// The server object used to pull data from the server
var server;

function preload () {
    // download all sprites
    game.load.atlas("objects", "/Content/Images/spritesheet.png", "/Content/Images/spritesheet.json");
    //server = new Server("", onerror, onclose);
}

function create () {
    // add all objects to game to be displayed
    //grass = new game.add.sprite(game.world.centerX, 500, 'grass', 'objects');
    grass = game.add.tileSprite(0, 0, game.world.width, game.world.height, "objects", "grass");
    CD.screen = new CD.LoadingScreen();
    CD.screen.create();
}

function update () {
    // called every frame
    CD.screen.update();
}

function render () {
    // 
}

function registerScreen(screen){
    game.state = {
        preload: preload,
        create: create,
        update: screen.update,
        render: screen.render
    };
}

function Server(address, onerror, onclose) {

    this.serverLocation = address;
    this.messages = new Array();

    this.socket = new WebSocket(address);
    this.socket.onerror = onerror;
    this.socket.onclose = onclose;
    this.socket.onmessage = function (evt) {
        this.messages.push(evt.data);
    }

    this.next = function () {
        return this.messages.pop();
    }

}

var incoming = {
    "interesting" : true
};