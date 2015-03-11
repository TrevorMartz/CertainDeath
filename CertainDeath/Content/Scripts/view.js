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
        this.subscribesTo = Array();
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
        this.subscribesTo = ["CurrentTile.Squares", "CurrentTile.Monsters", "CurrentTile.Buildings", "updates"];
        
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
                for (var x = 0; x < this.tiles.length; ++x) {
                    for (var y = 0; y < this.tiles[x].length; ++y) {
                        if (this.tiles[x][y] && this.tiles[x][y].destroy) {
                            this.tiles[x][y].destroy();
                        }
                        delete this.tiles[x][y];
                    }
                }
                for (var x = 0; x < this.resources.length; ++x) {
                    for (var y = 0; y < this.resources.length; ++y) {
                        if(this.resources[x][y] && this.resources[x][y].destroy)
                            this.resources[x][y].destroy();
                        delete this.resources[x][y];
                    }
                }
                for (x in this.monsters) {
                    if (this.monsters[x] && this.monsters[x].sprite.destroy)
                        this.monsters[x].sprite.destroy();
                    delete this.monsters[x];
                }
                if (this._placeState) {
                    this._placeState.sprite.destroy();
                    delete this._placeState;
                }

                if (this.fireOfLife.sprite && this.fireOfLife.sprite.destroy)
                    this.fireOfLife.sprite.destroy();
            }
        },
        onmessage: {
        	value: function (msg, property) {
            	if (msg != 'undefined') {
            		var tileSize = Math.min(this.height / this.squaresHigh, this.width / this.squaresWide);

                	this.UpdateMonsterPosition = function (id, xpos, ypos) {
                		var monster = this.monsters[id];
                		monster.sprite.x = Math.round(xpos / 32 * tileSize + this.boardX + monster.sprite.width / 2 * (monster.direction.X === "LEFT" ? 1 : -1) + (monster.direction.X == undefined ? 0 : (monster.direction.X === "LEFT" ? 15 : -15)));
                		monster.sprite.y = Math.round(ypos / 32 * tileSize + this.boardY - monster.sprite.height / 2 + (monster.direction.Y === "UP" ? 25 : 0));
                		monster.g.x = monster.sprite.x;
                		monster.g.y = monster.sprite.y;
                		this.monsters[id] = monster;
                	}

                	this.PlaceMonster = function (id, xpos, ypos, type, direction, status) {
                		var monster = {};
                		monster.name = type;
                		monster.direction = direction;
                		monster.sprite = game.add.sprite(xpos / 32 * tileSize + this.boardX, ypos / 32 * tileSize + this.boardY, "monsters");
                		monster.status = status;
                		monster.x = xpos;
                		monster.y = ypos;
                		monster.g = game.add.graphics(xpos, ypos - 7);

                		monster.drawHealth = function (max, current) {
                		    monster.g.clear();
                		    monster.g.beginFill(0xFFFFFF);
                		    monster.g.drawRect(0, 0, this.sprite.width, 5);
                		    monster.g.endFill();
                		    monster.g.beginFill(0xFF0000);
                		    monster.g.drawRect(1, 1, (this.sprite.width - 2) * (current/max), 3);
                		    monster.g.endFill();
                		}
                		monster.drawHealth(1,1);
                		this.monsters[id] = monster;
                		this.UpdateMonsterPosition(id, xpos, ypos);
                		this.UpdateMonsterStatus(id, status);
                	}

                	this.RemoveMonster = function (id) {
                		var monster = this.monsters[id];
                		monster.sprite.destroy();
                	}

                	this.UpdateMonsterStatus = function (id, status) {
                		var monster = this.monsters[id];

                		if (monster.status !== status) {
                			monster.sprite.animations.stop();
                		}

                		monster.status = status;
                		if (monster.direction.X === "LEFT") {
                			monster.sprite.anchor.setTo(1, 0);
                			monster.sprite.scale.x = -2; //flipped
                			monster.sprite.animations.add(status,
								monsterMap[monster.name + "/" + status + (status == "DYING" ? "" : "_" + (monster.direction.Y != "NONE" ? monster.direction.Y + "_" : "") + "RIGHT")],
								5, true);
                		}
                		else {
                			monster.sprite.scale.x = 2;
                			monster.sprite.animations.add(status,
								monsterMap[monster.name + "/" + status + (status == "DYING" ? "" : "_" + (monster.direction.Name))], 5, true);
                		}

                		if (status == "DYING") {
                			monster.sprite.animations.play(status, 5, false, true);//.onComplete(new Signal(function () { monster.sprite.destroy() }));
                		}
                		else
                			monster.sprite.animations.play(status);
                	}

                	this.PlaceBuilding = function (id, xpos, ypos, type) {

                		var sprite = game.add.sprite(xpos / 32 * tileSize + this.boardX / 2, ypos / 32 * tileSize + this.boardY / 2,
									"objects", "Fire");
                		this.buildings[i] = sprite;
                	}

/*Squares*/        if (property === "CurrentTile.Squares") {

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
/*Monsters*/        } else if (property === "CurrentTile.Monsters") {
						
                        for (var x = 0; x < msg.length; ++x) {
                        	var monster = this.monsters[msg[x].Id];
                        	if (monster === undefined) {
                        		monster = {};
							}
                            var positions = msg[x].Position.split(",");
                            var xpos = parseFloat(positions[0]);
                            var ypos = parseFloat(positions[1]);
                            monster.name = msg[x].Name;
                            monster.direction = msg[x].Direction;
                            var status = msg[x].Status;
                            var newAnimation = false;

							// if monster exists
							if (this.monsters[msg[x].Id] != undefined) {
								this.UpdateMonsterPosition(msg[x].Id, xpos, ypos);
								monster = this.monsters[msg[x].Id];

								// if monster has changed state
								if (monster.sprite.animations.currentAnim.name !== status) {
									newAnimation = true;
								}

							}
							else /*Monster does not exist yet*/ {
								this.PlaceMonster(msg[x].Id, xpos, ypos, monster.name, monster.direction);
                            	newAnimation = true;
							}

							if (newAnimation) {
								this.UpdateMonsterStatus(msg[x].Id, status);
							}

                        } // end for each monster
                    } // end if monster property
/*Buildings*/         else if (property === "CurrentTile.Buildings") {
						// this is not right and needs to be changed. this was just to get something on the screen
                    	if (msg[0] !== undefined) {
                    		
                    	}
                    } // end if building property
/*updates*/		  else if (property === "updates") {
                    	for (var x = 0; x < msg.length; ++x) {
                    		var update = msg[x];
                    		var type = update.UType;
                    		var id = update.ObjectId;

	  /*PlaceMonster*/     	if ("PlaceMonster" === type) {
	                   			if (this.monsters[id] == undefined) 
	  								this.PlaceMonster(id, update.PosX, update.PosY, update.Type, update.Direction, update.State);
	  /*Move*/  			} else if ("Move" === type) {
                    			if(this.monsters[id] != undefined) {
                    				this.UpdateMonsterPosition(id, update.MoveX, update.MoveY);
                    			}
	  /*MonsterState*/  	} else if ("MonsterState" === type) {
                    			this.UpdateMonsterStatus(id, update.State);
	  /*Remove*/  			} else if ("Remove" === type) {
	                   			if (this.monsters[id] !== "undefined") {
                    				this.RemoveMonster(id);
                    			}
                    			else {
									// remove building
                    			}
	  /*Health*/			} else if ("Health" === type) {
	                          if (this.monsters[id]) {
	                              this.monsters[id].drawHealth(update.HealthPoints, update.MaxHealthPoints);
	                          }
	  /*AddResouce*/  		} else if ("AddResourceToPlayer" === type) {
	                            //this.resources[update["ResourceType"]] += update["Amount"];
	  /*GameOver*/  		} else if ("GameOver" === type) {
                    			game.world.forEach(function (child) {  if(child.animations != undefined) child.animations.stop() }, this, true)
                    			alert("Game Over");
	  /*BuildingState*/		} else if ("BuildingState" === type) {

	  /*PlaceBuilding*/ 	} else if ("PlaceBuilding" === type) {

	  /*RemoveResource*/	} else if ("RemoveResourceFromSquare" === type) {

	  /*UpdateCost*/		} else if ("UpdateCost" === type) {

	  /*Upgrade*/    		} else if ("Upgrade" === type) {

	  /*World*/   			} else if ("World" === type) {

                    		}
                    	}
                    }
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
        this.subscribesTo = ["Player.Resources", "updates"];
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
            value: function (msg, prop) {
                if (prop == "updates") {
                    for (var x in msg) {
                        if (msg[x].UType && msg[x].UType == "AddResourceToPlayer") {
                            this.values[msg[x].ResourceType] += msg[x].Amount;
                        }
                    }
                } else {
                    this.values = msg;
                }
            }
        }
    });

    function ButtonScreen(game, x, y, width, height, group, key, callback, onmessagecallback) {
        Screen.call(this, x, y, width, height);
        this.game = game;
        this.button = this.game.add.button(x, y, group, callback);
        this.button.antialias = false;
        this.button.setFrames(key, key, key, key);
        this.button.scale.setTo(width / this.button.width, height / this.button.height);
        this.button.input.useHandCursor = true;
        this.visible = true;
        this.onmessagecallback = onmessagecallback;
        this.subscribesTo = ["BuildingCostsForTheWorld"];
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
        },
        onmessage: {
            value: function (data) {
                ScreenContainer.prototype.onmessage.call(this, data);
                if (this.onmessagecallback)
                    this.onmessagecallback(data);
                //UpdateShopCosts(data);
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
        this.subscribesTo = ["BuildingCostsForTheWorld"];
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
        },
        onmessage: {
            value: function (data) {
                ScreenContainer.prototype.onmessage.call(this, data);
                UpdateShopCosts(data);
            }
        }
    });

    function TileNavigator(game, server, x, y, width, height) {
        Screen.call(this, x, y, width, height);
        this.subscribesTo = ["UNKNOWN"];
        this.game = game;
        this.server = server;
        this.mainGameScreen = new MainGameScreen(game, server, x + 35, y + 35, width - 75, height - 75);
        this.server.register(this.mainGameScreen);
    }

    TileNavigator.prototype = Object.create(Screen.prototype, {
        create: {
            value: function () {
                this.disposables = [];

                Screen.prototype.create.call(this);
                this.mainGameScreen.create();
                var sprite;
                var g = this.game.add.graphics(this.x, this.y);
                g.beginFill(0xFF9900);
                g.moveTo(0, this.height / 2);
                g.lineTo(30, this.height / 2 + 30);
                g.lineTo(30, this.height / 2 - 30);
                g.lineTo(0, this.height / 2);
                g.endFill();
                sprite = this.game.add.sprite(0, 0);
                sprite.addChild(g);
                sprite.inputEnabled = true;
                sprite.events.onInputDown.add(this.onclick, { value: "left", context: this });
                sprite.input.useHandCursor = true;
                this.disposables.push(g);
                this.disposables.push(sprite);

                g.beginFill(0xFF9900);
                g.moveTo(this.width, this.height / 2);
                g.lineTo(this.width - 30, this.height / 2 + 30);
                g.lineTo(this.width - 30, this.height / 2 - 30);
                g.lineTo(this.width, this.height / 2);
                g.endFill();
                sprite = this.game.add.sprite(0, 0);
                sprite.addChild(g);
                sprite.inputEnabled = true;
                sprite.events.onInputDown.add(this.onclick, { value: "right", context: this });
                sprite.input.useHandCursor = true;
                this.disposables.push(g);
                this.disposables.push(sprite);

                g = this.game.add.graphics(this.x, this.y);
                g.beginFill(0xFF9900);
                g.moveTo(this.width / 2, 0);
                g.lineTo(this.width / 2 + 30, 30);
                g.lineTo(this.width / 2 - 30, 30);
                g.lineTo(this.width / 2, 0);
                sprite = this.game.add.sprite(0, 0);
                sprite.addChild(g);
                sprite.inputEnabled = true;
                sprite.events.onInputDown.add(this.onclick, { value: "up", context: this });
                sprite.input.useHandCursor = true;
                this.disposables.push(g);
                this.disposables.push(sprite);

                g = this.game.add.graphics(this.x, this.y);
                g.beginFill(0xFF9900);
                g.moveTo(this.width / 2, this.height);
                g.lineTo(this.width / 2 + 30, this.height - 30);
                g.lineTo(this.width / 2 - 30, this.height - 30);
                g.lineTo(this.width / 2, this.height);
                sprite = this.game.add.sprite(0, 0);
                sprite.addChild(g);
                sprite.inputEnabled = true;
                sprite.events.onInputDown.add(this.onclick, { value: "down", context: this });
                sprite.input.useHandCursor = true;
                this.disposables.push(g);
                this.disposables.push(sprite);

            }
        },
        update: {
            value: function () {
                Screen.prototype.update.call(this);
                this.mainGameScreen.update();
            }
        },
        destroy: {
            value: function () {
                Screen.prototype.destroy(this);
                this.mainGameScreen.destroy();
                this.g.destroy();
                if (this.disposables) {
                    for (var x = 0; x < this.disposables.length; ++x) {
                        this.disposables[x].destroy();
                    }
                }
            }
        },
        onmessage: {
            value: function (msg) {
                // TODO
            }
        },
        onclick: {
            value: function (click) {
                this.context.mainGameScreen.destroy();
                delete this.context.mainGameScreen;
                this.context.mainGameScreen = new MainGameScreen(this.context.game, this.context.server, this.context.x + 35, this.context.y + 35, this.context.width - 75, this.context.height - 75);
                this.context.server.register(this.context.mainGameScreen);

                this.context.server.send(JSON.stringify({
                    event: "moveTile",
                    direction: this.value
                }));
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
        TileNavigator: TileNavigator,
        current: new Screen(0, 0, 100, 100)
    }
})();