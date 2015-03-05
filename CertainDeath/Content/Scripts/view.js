﻿/// <reference path="game.js" />
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
        this.subscribesTo = Array();
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
            if (this.g) {
                this.g.destroy();
            }
        },

        onmessage: function (obj){
            messages.push(obj);
        },

        render: function () {
            //if (this.g === undefined && this.game !== undefined) {
            //    var offset = 20;
            //    this.g = this.game.add.graphics(0, 0);

            //    var colors = [0xFF0000, 0x00FF00, 0x0000FF, 0xFF00FF, 0x00FFFF, 0xFFFF00];

            //    this.g.lineStyle(2, colors[Math.floor(Math.random() * 6)], 1);

            //    this.g.moveTo(this.x - offset, this.y);
            //    this.g.lineTo(this.x + this.width + offset, this.y);

            //    this.g.moveTo(this.x - offset, this.y + this.height);
            //    this.g.lineTo(this.x + this.width + offset, this.y + this.height);

            //    this.g.moveTo(this.x, this.y - offset);
            //    this.g.lineTo(this.x, this.y + this.height + offset);

            //    this.g.moveTo(this.x + this.width, this.y - offset);
            //    this.g.lineTo(this.x + this.width, this.y + this.height + offset);
            //}
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
    function MainGameScreen(game, server, x, y, screenWidth, screenHeight) {
        if (screenWidth === undefined)
            screenWidth = game.width;
        if (screenHeight == undefined)
            screenHeight = game.height;
        Screen.call(this, x, y, screenWidth, screenHeight);
        this.game = game;
        this.server = server;
        this.tiles = new Array();
        this.resources = new Array();
        this.monsters = new Array();
        this.fireOfLife = {};
        this._pointerDown = false;
        this.squaresWide = 20;
        this.squaresHigh = 20;
        this.boardX = 0;
        this.boardY = 0;
        this.subscribesTo = ["CurrentTile.Squares", "CurrentTile.Monsters", "CurrentTile.Buildings"];
        
        // Private variable?
        this._placeState = null;
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
                var tileSize = Math.min(this.height / 20, this.width / 20);
                if (game.input.activePointer.isDown && !this._pointerDown) {
                    this._pointerDown = true;
                    if(game.input.activePointer.x >= this.boardX &&
                        game.input.activePointer.x <= this.boardX + this.squaresWide * tileSize &&
                        game.input.activePointer.y >= this.boardY &&
                        game.input.activePointer.y <= this.boardY + this.squaresHigh * tileSize) {
                        if (this._placeState) {
                            // Send a request to place a building to the server
                            var request = {
                                "event": "placeBuilding",
                                "type": this._placeState.type,
                                "x": Math.floor((game.input.activePointer.x - this.boardX) / (tileSize)),
                                "y": Math.floor((game.input.activePointer.y - this.boardY) / (tileSize))
                            };
                            this.server.send(JSON.stringify(request));

                            this._placeState.sprite.destroy();
                            this._placeState = null;
                        } else {
                            // Send a click event to the server
                            this.server.send(JSON.stringify({
                                "event": "click",
                                "x": (game.input.activePointer.x - this.boardX) / (tileSize),
                                "y": (game.input.activePointer.y - this.boardY) / (tileSize)
                            }));
                        }
                    }
                } else if (this._placeState &&
                        game.input.activePointer.x >= this.boardX &&
                        game.input.activePointer.x <= this.boardX + this.squaresWide * tileSize &&
                        game.input.activePointer.y >= this.boardY &&
                        game.input.activePointer.y <= this.boardY + this.squaresHigh * tileSize) {
                    this._placeState.sprite.x = game.input.activePointer.x - ((game.input.activePointer.x - this.x) % tileSize);
                    this._placeState.sprite.y = game.input.activePointer.y - ((game.input.activePointer.y - this.y) % tileSize);
                }

                if (game.input.activePointer.isUp && this._pointerDown) {
                    this._pointerDown = false;
                }
            }
        },
        destroy: {
            value: function () {
                //FIXME: I think the foreach here will not behave as expected
                for (var x = 0; x < this.tiles.length; ++x) {
                    if(this.tiles[x].destroy)
                        this.tiles[x].destroy();
                }
                for (var x = 0; x < this.resources.length; ++x) {
                    if(this.resources[x].destroy)
                        this.resources[x].destroy();
                }
                if (this._placeState) {
                    this._placeState.sprite.destroy();
                    delete this._placeState;
                }
            }
        },
        onmessage: {
            value: function (msg, property) {
                if (msg != 'undefined') {

                    if (property === "CurrentTile.Squares") {
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
                                if (this.tiles[i] === undefined) {
                                    this.tiles[i] = new Array();
                                }

                                if (this.tiles[i][j] === undefined || this.tiles[i][j] === null) {
                                    //var rand = Math.ceil(Math.random() * 3);
                                    var sprite = game.add.sprite(i * tileSize + this.boardX, j * tileSize + this.boardY,
                                        "objects", msg[i][j].TypeName);
                                    sprite.scale.setTo(tileSize / sprite.width);
                                    this.tiles[i][j] = sprite;
                                }

                                if (this.resources[i] === undefined) {
                                    this.resources[i] = new Array();
                                }

                                if (msg[i][j].ResourceName !== undefined) {
                                    if (this.resources[i][j] !== undefined && msg[i][j] != this.resources[i][j].key) {
                                        this.resources[i][j].destroy();
                                        this.resources[i][j] = null;
                                    }

                                    if (this.resources[i][j] === undefined || this.resources[i][j] === null) {
                                        var sprite2 = game.add.sprite(i * tileSize + this.boardX, j * tileSize + this.boardY,
                                            "objects", msg[i][j].ResourceName);
                                        sprite2.scale.setTo(tileSize / sprite2.width);
                                        this.resources[i][j] = sprite2;
                                    }
                                } else if(this.resources[i][j] != undefined){
                                    this.resources[i][j].destroy();
                                    delete this.resources[i][j];
                                }
                            } // end for
                        } // end for
                    } else if (property === "CurrentTile.Monsters") {
                        var tileSize = Math.min(this.height / this.squaresHigh, this.width / this.squaresWide);
                        for (var x = 0; x < msg.length; ++x) {
                            var positions = msg[x].Position.split(",");
                            var xpos = parseFloat(positions[0]);
                            var ypos = parseFloat(positions[1]);
                            var name = msg[x].Name;
                            var direction = msg[x].Direction;
                            var status = msg[x].Status;
                            var newAnimation = false;

                            var sprite = null;
								// if monster exists
								if (this.monsters[msg[x].Id] !== undefined) {
									sprite = this.monsters[msg[x].Id];
									sprite[0].x = Math.round(xpos / 32 * tileSize + this.boardX + sprite[0].width / 2 * (direction.X === "LEFT" ? 1 : -1) + (direction.X == undefined ? 0 : (direction.X === "LEFT" ? 15 : -15)));
									sprite[0].y = Math.round(ypos / 32 * tileSize + this.boardY - sprite[0].height / 2 + (direction.Y === "UP" ? 25 : 0));
									sprite[1].x = xpos / 32 * tileSize + this.boardX;
									sprite[1].y = ypos / 32 * tileSize + this.boardY;
									// if monster has changed state
									if (sprite[0].animations.currentAnim.name !== status) {
										sprite[0].animations.stop();
										newAnimation = true;
									}
								} else /*Monster does not exist yet*/ {
                            		sprite = [game.add.sprite(xpos / 32 * tileSize + this.boardX, ypos / 32 * tileSize + this.boardY,
										"monsters"), game.add.graphics(xpos / 32 * tileSize + this.boardX, ypos / 32 * tileSize + this.boardY)];
                            		sprite[0].x = Math.round(sprite[0].x + sprite[0].width / 2 * (direction.X === "LEFT" ? 1 : -1) + (direction.X == undefined ? 0 : (direction.X === "LEFT" ? 15 : -15)));
                            		sprite[0].Y = Math.round(sprite[0].Y - sprite[0].height / 2 + (direction.Y ==="UP" ? 25 : 0));
                            		sprite[1].beginFill(0x000000, 0.5);
                            		sprite[1].drawCircle(0, 0, 15);
                            		this.monsters[msg[x].Id] = sprite;
                            		newAnimation = true;
								}

							if (newAnimation) {
								if (direction.X === "LEFT") {
									sprite[0].anchor.setTo(1, 0); 
									sprite[0].scale.x = -2; //flipped
									sprite[0].animations.add(status,
									monsterMap[name + "/" + status + "_" + (direction.Y != "NONE" ? direction.Y + "_" : "") + "RIGHT"],
									5, true);
								}
								else {
									sprite[0].scale.x = 2;
									sprite[0].animations.add(status,
									monsterMap[name + "/" + status + "_" + (direction.Name)],
									5, true);
								}
								sprite[0].animations.play(status);
							} // end if newAnimation
                        } // end for each monster
                    } // end if monster property
                    else if (property === "CurrentTile.Buildings") {
                    	if (msg[0] !== undefined) {
                    		var tileSize = Math.min(this.height / this.squaresHigh, this.width / this.squaresWide);
                    		var positions = msg[0].Position.split(",");
                    		var xpos = parseFloat(positions[0]);
                    		var ypos = parseFloat(positions[1]);
                    		this.fireOfLife.sprite = game.add.sprite(xpos / 32 * tileSize + this.boardX, ypos / 32 * tileSize + this.boardY,
										"objects", "Fire");
                    	}
                    } // end if building property
                } // end if
            } // end func
        },
        placeBuilding: {
            /**
             * @param type = the name of the type of building to be placed
             */
            value: function (type) {
                console.log(type + " being placed.");
                this._placeState = {};
                this._placeState.sprite = this.game.add.sprite(this.x, this.y, "objects", "Fire"); //type);
                this._placeState.type = type;
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
                Screen.prototype.create.call(this);
                for (var x = 0; x < this.screens.length; ++x) {
                    this.screens[x].create();
                }
            }
        },
        update: {
            value: function () {
                Screen.prototype.update.call(this);
                for (var x = 0; x < this.screens.length; ++x) {
                    this.screens[x].update();
                }
            }
        },
        click: {
            value: function (evt) {
                Screen.prototype.click.call(this);
                for (var x = 0; x < this.screens.length; ++x) {
                    this.screens[x].click();
                }
            }
        },
        destroy: {
            value: function () {
                Screen.prototype.destroy.call(this);
                for (var x = 0; x < this.screens.length; ++x) {
                    this.screens[x].destroy();
                }
            }
        },
        render: {
            value: function () {
                Screen.prototype.render.call(this);
                for (var x = 0; x < this.screens.length; ++x) {
                    this.screens[x].render();
                }
            }
        }
    });

    // Height is set
    function InventoryBar(game, x, y, width, height) {
        Screen.call(this, x, y, width, height);
        this.values = {
            "WOOD": 0,
            "IRON": 0,
            "CORN": 0,
            "STONE": 0,
            "COAL": 0
        };
        this.game = game;
        this.resourceTypes = ["COAL", "IRON", "WOOD", "CORN", "STONE"];
        this.subscribesTo = ["Player.Resources"];
        this.sprites = Array();
        this.resourceText = Array();
    }

    InventoryBar.prototype = Object.create(Screen.prototype, {
        create: {
            value: function () {
                // Not much we can do here...
                Screen.prototype.create.call(this);

                this.g = this.game.add.graphics(this.x, this.y);
                this.g.beginFill(0xFFFFFF, 1);
                this.g.drawRect(0, 0, this.width, this.height);

                var valueWidth = this.width / this.resourceTypes.length;
                for (var x = 0; x < this.resourceTypes.length; ++x){
                    var sprite = game.add.sprite(this.x + x * valueWidth + valueWidth / 2 - 32 - 4, this.height * 0.5 - 0.5 * 32 + this.y, "objects", this.resourceTypes[x]);
                    this.resourceText[this.resourceTypes[x]] = game.add.text(this.x + x * valueWidth + valueWidth / 2 + 4, this.height * 0.5 - 0.5 * 32 + this.y, "0");
                    this.sprites.push(sprite);
                }
            }
        },
        update: {
            value: function () {
                Screen.prototype.update.call(this);

                for (var type in this.values) {
                    this.resourceText[type].setText(this.values[type]);
                }
            }
        },
        //click: {
        //    value: function (evt) {
                
        //    }
        //},
        destroy: {
            value: function () {
                Screen.prototype.destroy.call(this);
                for (var x = 0; x < this.sprites.length; ++x)
                    this.sprites[x].destroy();
                for (var x in this.resourceText)
                    this.resourceText[x].destroy();
                this.g.destroy();
            }
        },
        onmessage: {
            value: function (msg) {
                this.values = msg;
            }
        }
    });

    function ButtonScreen(game, x, y, width, height, group, key, callback) {
        Screen.call(this, x, y, width, height);
        this.game = game;
        this.button = this.game.add.button(x, y, group, callback);
        this.button.antialias = false;
        this.button.setFrames(key, key, key, key);
        this.button.scale.setTo(width / this.button.width, height / this.button.height);
        this.button.input.useHandCursor = true;
        this.visible = true;
    }

    ButtonScreen.prototype = Object.create(Screen.prototype, {
        destroy: {
            value: function () {
                Screen.prototype.destroy.call(this);
                this.button.destroy();
            }
        },
        update: {
            value : function(){
                Screen.prototype.update.call(this);
                this.button.visible = this.visible;
            }
        }
    });

    function TextScreen(game, x, y, width, height, text, transparent) {
        Screen.call(this, x, y, width, height);
        this.text = text;
        this.transparent = transparent;
        this.game = game;
    }

    TextScreen.prototype = Object.create(Screen.prototype, {
        create: {
            value: function () {
                Screen.prototype.create.call(this);
                this.textO = game.add.text(this.x + this.width / 2, this.y + this.height / 2, this.text);
                if (!this.transparent) {
                    this.g = game.add.graphics(this.x, this.y);
                    this.g.beginFill(0xFFFFFF);
                    this.g.drawRect(0, 0, this.width, this.height);
                }
            }
        },
        destroy: {
            value: function () {
                Screen.prototype.destroy.call(this);
                if (this.textO) {
                    this.textO.destroy();
                }
                if (this.g) {
                    this.g.destroy();
                }
            }
        }
    });

    /**
     * Ignore this doc. It's wrong.
     * @param game = the instance of Phaser.Game
     * @param server = the instance of Server that pulls data from the server
     * @param x = the x coordinate of the top left corner of this view
     * @param y = the y coordinate of the top left corner of this view
     * @param width = the width of this view
     * @param height = the height of this view
     * @param funcs = an object that contains references to functions called
     *              during the lifecycle of the game. The list of functions
     *              includes:
     *               - openShop()
     *               - openUpgradeWindow()
     */
    function BuildingShop(game, server, x, y, width, height, funcs) {
        ScreenContainer.call(this, new Array(), x, y, width, height);
        this.game = game;
        this.server = server;
        this.subscribesTo = "BuildingPrices";
        this.visible = false;
    }

    BuildingShop.prototype = Object.create(ScreenContainer.prototype, {
        create: {
            value: function () {
                ScreenContainer.prototype.create.call(this);
                this.g = game.add.graphics(this.x, this.y);
                this.g.beginFill(0x333333, 0.9);
                this.g.drawRect(0, 0, this.width, this.height);
                this.g.visible = false;
                this.game.world.bringToTop(this.g);
            }
        },
        update: {
            value: function () {
                ScreenContainer.prototype.update.call(this);
                // Do nothing.
            }
        }
    });

    return {
        MainGameScreen: MainGameScreen,
        LoadingScreen: LoadingScreen,
        ScreenContainer: ScreenContainer,
        InventoryBar: InventoryBar,
        BuildingShop: BuildingShop,
        ButtonScreen: ButtonScreen,
        Screen: Screen,
        TextScreen: TextScreen,
        current: new Screen(0, 0, 100, 100)
    }
})();