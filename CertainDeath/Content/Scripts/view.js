/// <reference path="game.js" />
View = (function () {

    /**
     * The base screen class
     **/
    function Screen(x, y, width, height) {
        // The coordinates of the screen
        if (x === undefined)
            x = 0;
        if (y === undefined)
            y = 0;
        if (width === undefined)
            throw new Error("width must be defined.");
        if (height === undefined)
            throw new Error("height must be defined.");
        this.x = x;
        this.y = y;
        this.width = width;
        this.height = height;
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
    function LoadingScreen(game, x, y, width, height) {
        width = width || game.width;
        height = height || game.height;
        Screen.call(x, y, width, height);
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
    function MainGameScreen(game, server, x, y, width, height) {
        width = width || game.width;
        height = height || game.height;
        Screen.call(x, y, width, height);
        this.game = game;
        this.server = server;
        this.objects = new Array();
        this._pointerDown = false;
        this.squaresWide = 0;
        this.squaresHigh = 0;
        this.boardX = 0;
        this.boardY = 0;
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

                        this.boardX = (this.width - this.squaresWide * 32) / 2 + this.x;
                        this.boardY = (this.height - this.squaresHigh * 32) / 2 + this.y;

                        for (var i = 0; i < msg.Squares.length; i++) {
                            for (var j = 0; j < msg.Squares[i].length; j++) {
                                objects = new Array();
                                var rand = Math.ceil(Math.random() * 3);
                                objects += game.add.sprite(i * 32 + this.boardX, j * 32 + this.boardY,
                                    "objects", msg.Squares[i][j].TypeName + rand);
                                if (msg.Squares[i][j].ResourceName !== undefined) {
                                    console.log(msg.Squares[i][j].ResourceName);
                                    objects += game.add.sprite(i * 32 + this.boardX, j * 32 + this.boardY,
                                        "objects", msg.Squares[i][j].ResourceName);
                                }
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

    /**
     * A ContainerScreen takes an array of objects that contain a screen,
     * and x and y coordinates on the screen. Provide the screen objects
     * in the order that you want their lifecycle methods to be called.
     */
    function ScreenContainer(screens, x, y, width, height) {
        Screen.call(x, y, width, height);

        if (!(typeof (screens) === typeof (Array) && screen in screens[0] && x in screens[0] && y in screens[0])) {
            throw new Error("The object passed into GameScreen did not match the expected format.")
        }

        this.screens = screens;
    }

    ScreenContainer.prototype = Object.create(Screen.prototype, {
        create: {
            value: function () {
                for (var x in screens) {
                    x.screen.create();
                }
            }
        },
        update: {
            value: function () {
                for (var x in screens) {
                    x.screen.update();
                }
            }
        },
        click: {
            value: function (evt) {
                for (var x in screens) {
                    x.screen.click();
                }
            }
        },
        destroy: {
            value: function () {
                for (var x in screens) {
                    x.screen.destroy();
                }
            }
        }
    });

    return {
        MainGameScreen: MainGameScreen,
        LoadingScreen: LoadingScreen,
        ScreenContainer: ScreenContainer,
        Screen: Screen,
        current: new Screen()
    }
})();