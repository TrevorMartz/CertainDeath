/// <reference path="game.js" />
View = (function () {

    /**
     * The base screen class
     **/
    function Screen() {
        // The basic screen doesn't store any data
    }
    Screen.prototype = {
        /**
         * Called when the screen is first created. Use to load graphics and such.
         */
        create: function () { },

        update: function () { },

        click: function (evt) { },

        destroy: function () { }
    }
    Screen.prototype.constructor = Screen;

    /**
     * The loading screen. Requires access to the game object.
     */
    function LoadingScreen(game) {
        this.text = null;
        this.graphics = null;
        this.game = game;
    }
    LoadingScreen.prototype = Object.create(Screen.prototype, {
        create: {
            value: function () {
                this.graphics = game.add.graphics(0, 0);
                this.graphics.beginFill(0xffffff);
                this.graphics.drawRect(0, 0, game.width, game.height);
                this.text = game.add.text(game.world.centerX, game.world.centerY, "loading...");
                this.text.anchor.setTo(0.5);
            }
        },
        destroy: {
            value: function () {
                this.text.destroy();
                this.graphics.destroy();
            }
        }
    });
    LoadingScreen.prototype.constructor = LoadingScreen;

    /**
     * The main game screen.
     * @param game - A Phaser.js game object
     * @param server - A Server object to send and recieve 
     */
    function MainGameScreen(game, server){
        this.game = game;
        this.server = server;
        this.objects = new Array();
        this._pointerDown = false;
        this.squaresWide = 0;
        this.squaresHigh = 0;
    }
    MainGameScreen.prototype = Object.create(Screen.prototype, {
        create: {
            value: function(){
                
            }
        },
        update: {
            value: function () {
                var msg = undefined;
                while ((msg = this.server.next()) != undefined) {
                    // Updating background...
                    if (msg.Squares != 'undefined') {
                        this.squaresWide = msg.Squares.length;
                        this.squaresHigh = msg.Squares[0].length;
                        for (var i = 0; i < msg.Squares.length; i++) {
                            for (var j = 0; j < msg.Squares[i].length; j++) {
                                objects = new Array();
                                objects += game.add.tileSprite(i * (game.world.width/msg.Squares.length), j * (game.world.height/msg.Squares[i].length),
                                    game.world.width / msg.Squares.length, game.world.height / msg.Squares[i].length,
                                    "objects", msg.Squares[i][j].TypeName);
                            }
                        }
                    }
                }

                // Now check the user's input
                if (game.input.activePointer.isDown && !this._pointerDown) {
                    this._pointerDown = true;
                    Server.send(JSON.stringify({
                        "event": "click",
                        "x": game.input.activePointer.x / (game.world.width / this.squaresWide),
                        "y": game.input.activePointer.y / (game.world.height / this.squaresHigh)
                    }));
                }

                if (game.input.activePointer.isUp && this._pointerDown) {
                    this._pointerDown = false;
                }
            }
        },
        destroy: {
            value: function () {
                for (obj in objects) {
                    obj.destroy();
                }
            }
        }
    });

    return {
        MainGameScreen: MainGameScreen,
        LoadingScreen: LoadingScreen,
        Screen: Screen,
        current: new Screen()
    }
})();