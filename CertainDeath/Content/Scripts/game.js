/// <reference path="phaser.js" />

var game = new Phaser.Game("", "", Phaser.AUTO, "body", { preload: preload, create: create, update: update, render: render });

var buildings;
var resources;
var monsters;

var preload = new function () {
    // download all sprites
    game.load.atlas();
}

var create = new function () {
    // add all objects to game to be displayed
}

var update = new function () {
    // called every frame

}

var render = new function () {
    // 
}