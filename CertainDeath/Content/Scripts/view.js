/// <reference path="game.js" />
View = (function () {

    /**
     * The base screen class
     **/
    function Screen(x, y, screenWidth, screenHeight) {
        // The coordinates of the screen
        if (x === undefined)
            x = 0;
        if (y === undefined)
            y = 0;
        if (screenWidth === undefined)
            throw new Error("width must be defined.");
        if (screenHeight === undefined)
            throw new Error("height must be defined.");
        this.x = x;
        this.y = y;
        this.width = screenWidth;
        this.height = screenHeight;
        this.subscribesTo = null;
        this.messages = Array();
    }
    Screen.prototype = {
        /**
         * Called when the screen is first created. Use to load graphics and such.
         */
        create: function () { },

        update: function () {
            
        },

        click: function (evt) { },

        destroy: function () {
            this.g.destroy();
        },

        onmessage: function (obj){
            messages.push(obj);
        },

        render: function () {
            if (this.g === undefined && this.game !== undefined) {
                var offset = 20;
                this.g = this.game.add.graphics(0, 0);

                this.g.lineStyle(2, 0x00FF00, 1);

                this.g.moveTo(this.x - offset, this.y);
                this.g.lineTo(this.x + this.width + offset, this.y);

                this.g.moveTo(this.x - offset, this.y + this.height);
                this.g.lineTo(this.x + this.width + offset, this.y + this.height);

                this.g.moveTo(this.x, this.y - offset);
                this.g.lineTo(this.x, this.y + this.height + offset);

                this.g.moveTo(this.x + this.width, this.y - offset);
                this.g.lineTo(this.x + this.width, this.y + this.height + offset);
            }
        }
    }
    Screen.prototype.constructor = Screen;

    /**
     * The loading screen. Requires access to the game object.
     */
    function LoadingScreen(game, x, y, width, height) {
        if(width === undefined)
            width = game.width;
        if(height == undefined)
            height = game.height;
        Screen.call(this, x, y, width, height);
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
    function MainGameScreen(game, x, y, screenWidth, screenHeight) {
        if (screenWidth === undefined)
            screenWidth = game.width;
        if (screenHeight == undefined)
            screenHeight = game.height;
        Screen.call(this, x, y, screenWidth, screenHeight);
        this.game = game;
        this.server = server;
        this.objects = new Array();
        this._pointerDown = false;
        this.squaresWide = 0;
        this.squaresHigh = 0;
        this.boardX = 0;
        this.boardY = 0;
        this.subscribesTo = "Squares";
    }

    MainGameScreen.prototype = Object.create(Screen.prototype, {
        create: {
            value: function(){
                
            }
        },
        update: {
            value: function () {
                Screen.prototype.update.call(this);

                // Now check the user's input
                if (game.input.activePointer.isDown && !this._pointerDown) {
                    this._pointerDown = true;
                    Server.send(JSON.stringify({
                        "event": "click",
                        "x": (game.input.activePointer.x - this.x) / (this.width / this.squaresWide),
                        "y": (game.input.activePointer.y - this.y) / (this.height / this.squaresHigh)
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
        },
        onmessage: {
            value: function (msg) {
                if (msg != 'undefined') {
                    this.squaresWide = msg.length;
                    this.squaresHigh = msg[0].length;

                    var tileSize = Math.min(this.height / this.squaresHigh, this.width / this.squaresWide);

                    // Make the board snap to a good scale so that the images still look crisp
                    if (tileSize > 64)
                        tileSize = 64;
                    else if (tileSize > 32)
                        tileSize = 32;

                    this.boardX = (this.width - this.squaresWide * tileSize) / 2 + this.x;
                    this.boardY = (this.height - this.squaresHigh * tileSize) / 2 + this.y;

                    for (var i = 0; i < msg.length; i++) {
                        for (var j = 0; j < msg[i].length; j++) {
                            objects = new Array();
                            var rand = Math.ceil(Math.random() * 3);
                            var sprite = game.add.sprite(i * tileSize + this.boardX, j * tileSize + this.boardY,
                                "objects", msg[i][j].TypeName + rand);
                            sprite.scale.setTo(tileSize / sprite.width);
                            objects.push(sprite);

                            if (msg[i][j].ResourceName !== undefined) {
                                var sprite2 = game.add.sprite(i * tileSize + this.boardX, j * tileSize + this.boardY,
                                    "objects", msg[i][j].ResourceName);
                                sprite2.scale.setTo(tileSize / sprite2.width);
                                objects.push(sprite2);
                            }
                        }
                    }
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
        Screen.call(this, x, y, width, height);

        //if (!(screens === undefined && screen in screens[0] && x in screens[0] && y in screens[0])) {
        //    throw new Error("The object passed into GameScreen did not match the expected format.")
        //}

        this.screens = screens;
    }

    ScreenContainer.prototype = Object.create(Screen.prototype, {
        create: {
            value: function () {
                for (var x = 0; x < this.screens.length; ++x) {
                    this.screens[x].create();
                }
            }
        },
        update: {
            value: function () {
                for (var x = 0; x < this.screens.length; ++x) {
                    this.screens[x].update();
                }
            }
        },
        click: {
            value: function (evt) {
                for (var x = 0; x < this.screens.length; ++x) {
                    this.screens[x].click();
                }
            }
        },
        destroy: {
            value: function () {
                for (var x = 0; x < this.screens.length; ++x) {
                    this.screens[x].destroy();
                }
            }
        },
        render: {
            value: function () {
                for (var x = 0; x < this.screens.length; ++x) {
                    this.screens[x].render();
                }
            }
        }
    });

    // Height is set
    function InventoryBar(game, x, y, width, height) {
        Screen.call(this, x, y, width, height);
        this.values = Array();
        this.game = game;
    }

    InventoryBar.prototype = Object.create(Screen.prototype, {
        //create: {
        //    value: function () {
        //        // Not much we can do here...
        //    }
        //},
        update: {
            value: function () {
                Screen.prototype.update.call(this);
            }
        },
        //click: {
        //    value: function (evt) {
                
        //    }
        //},
        destroy: {
            value: function () {
                
            }
        },
        onmessage: {
            value: function (msg) {
                this.values = msg;
            }
        }
    });

    return {
        MainGameScreen: MainGameScreen,
        LoadingScreen: LoadingScreen,
        ScreenContainer: ScreenContainer,
        InventoryBar : InventoryBar,
        Screen: Screen,
        current: new Screen(0, 0, 100, 100)
    }
})();