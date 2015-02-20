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
    View.current = new View.MainGameScreen(game, Server);
    View.current.create();
}

function update () {
    // called every frame
    View.current.update();
}

function render () {
    // I don't know what this is for
    // Docs say it's not used often, so...
}

var Server = new WebSocket("wss://" + window.location.host + "/api/WebSocket/10");
Server.messages = new Array();
Server.onmessage = function (message) {
    this.messages.push(JSON.parse(message.data));
}
Server.next = function () {
    return this.messages.pop();
}