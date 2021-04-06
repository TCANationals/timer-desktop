const electron = require('electron')
const { app, Tray, Menu, dialog } = require('electron')
const path = require('path')

const Assets = require('./Assets')

class TrayGenerator {
  constructor(mainWindow) {
    this.tray = null
    this.mainWindow = mainWindow
  }

  showWindow = () => {
    this.mainWindow.setAlwaysOnTop(true, 'pop-up-menu')
    this.mainWindow.show();
    //this.mainWindow.setVisibleOnAllWorkspaces(true);
    //this.mainWindow.focus();
  }

  toggleWindow = () => {
    if (this.mainWindow.isVisible()) {
        this.mainWindow.hide();
    } else {
        this.showWindow();
    }
  }

  rightClickMenu = () => {
    const menu = [
        {
          label: 'Quit',
          click: () => {
            this.handleQuit()
          }
        }
      ];
      this.tray.popUpContextMenu(Menu.buildFromTemplate(menu));
  }

  handleQuit = () => {
    const options = {
      type: "question",
      buttons: ["&Yes","&No"],
      defaultId: 1,
      title: 'Quit Application?',
      message: 'Do you really want to quit?',
      //detail: 'Press Yes button to quit',
      cancelId: 1,
      noLink: true,
      normalizeAccessKeys: true
    }
  
    dialog.showMessageBox(null, options).then(res => {
      if (res.response === 0){
        //Yes button pressed
        app.quit()
      }
      else if (res.response === 1) {
        //No button pressed
      }
    })
  }

  createTray = () => {
    this.tray = new Tray(Assets.icon);
    this.tray.setIgnoreDoubleClickEvents(true);
    this.tray.on('click', this.toggleWindow);
    this.tray.on('right-click', this.rightClickMenu);
  }
}

module.exports = TrayGenerator
