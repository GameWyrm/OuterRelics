using System;
using UnityEngine;
using UnityEngine.UI;

namespace ScavengerHunt
{
    public interface IMenuAPI
    {
        /// <summary>
        /// Creates a button on the title screen with index index and text name, which opens the menu menuToOpen.
        /// </summary>
        /// <param name="name">Text of the button</param>
        /// <param name="index">Index of the button</param>
        /// <param name="menuToOpen">Which menu gets opened</param>
        /// <returns></returns>
        GameObject TitleScreen_MakeMenuOpenButton(string name, int index, Menu menuToOpen);
        /// <summary>
        /// Creates a button on the title screen with index index and text name, which loads the scene sceneToLoad. When the optional parameter confirmPopup is given, the button will first open this popup menu. If confirm is selected, the scene will load.
        /// </summary>
        /// <param name="name">Text of the button</param>
        /// <param name="index">Index of the button</param>
        /// <param name="sceneToLoad">Scene that will be loaded on click</param>
        /// <param name="confirmPopup">Popup that requires confirmation</param>
        /// <returns></returns>
        GameObject TitleScreen_MakeSceneLoadButton(string name, int index, SubmitActionLoadScene.LoadableScenes sceneToLoad, PopupMenu confirmPopup = null);
        /// <summary>
        /// Creates a button on the title screen with index index and text name. You can then do whatever you like with the button by doing (button).onClick.AddListener( ... ).
        /// </summary>
        /// <param name="name">Text of the button</param>
        /// <param name="index">Index of the button</param>
        /// <returns></returns>
        Button TitleScreen_MakeSimpleButton(string name, int index);
        /// <summary>
        /// Creates a button on the pause screen with index index and text name, which opens the menu menuToOpen.
        /// </summary>
        /// <param name="name">Text of the button</param>
        /// <param name="menuToOpen">Which menu gets opened</param>
        /// <param name="customMenu"></param>
        /// <returns></returns>
        GameObject PauseMenu_MakeMenuOpenButton(string name, Menu menuToOpen, Menu customMenu = null);
        /// <summary>
        /// Creates a button on the pause screen with index index and text name, which loads the scene sceneToLoad. When the optional parameter confirmPopup is given, the button will first open this popup menu. If confirm is selected, the scene will load.
        /// </summary>
        /// <param name="name">Text of the button</param>
        /// <param name="sceneToLoad">Scene to get loaded into</param>
        /// <param name="confirmPopup">Popup menu for confirmation</param>
        /// <param name="customMenu"></param>
        /// <returns></returns>
        GameObject PauseMenu_MakeSceneLoadButton(string name, SubmitActionLoadScene.LoadableScenes sceneToLoad, PopupMenu confirmPopup = null, Menu customMenu = null);
        /// <summary>
        /// Creates a button on the pause screen with index index and text name. You can then do whatever you like with the button by doing (button).onClick.AddListener( ... ).
        /// </summary>
        /// <param name="name">Text on the button</param>
        /// <param name="customMenu">Menu that the button opens</param>
        /// <returns></returns>
        Button PauseMenu_MakeSimpleButton(string name, Menu customMenu = null);
        /// <summary>
        /// Makes a sub-menu for the pause menu, with the name of title.
        /// </summary>
        /// <param name="title">Title of the sub-list</param>
        /// <returns></returns>
        Menu PauseMenu_MakePauseListMenu(string title);
        /// <summary>
        /// Makes a popup with a message and two button choices, typically YES/NO or CONFIRM/CANCEL. message is the main message of the popup, confirmText and cancelText are what appears on the two buttons.
        /// </summary>
        /// <param name="message">Text of the message</param>
        /// <param name="confirmText">Text for confirming</param>
        /// <param name="cancelText">Text for cancelling</param>
        /// <returns></returns>
        PopupMenu MakeTwoChoicePopup(string message, string confirmText, string cancelText);
        /// <summary>
        /// Makes a popup with a message, a text input field, and two button choices. Same parameters as MakeTwoChoicePopup, with the addition of placeholderText as what appears in the background of the input box before you begin typing.
        /// </summary>
        /// <param name="message">Text of the message</param>
        /// <param name="placeholderMessage">Message that appears in the textbox</param>
        /// <param name="confirmText">Text for confirming</param>
        /// <param name="cancelText">Text for cancelling</param>
        /// <returns></returns>
        PopupInputMenu MakeInputFieldPopup(string message, string placeholderMessage, string confirmText, string cancelText);
        /// <summary>
        /// Makes a popup with a message and only one button to close the popup. message is the main message of the popup, continueButtonText is what appears on the button - usually just "OK".
        /// </summary>
        /// <param name="message">Text of the message</param>
        /// <param name="continueButtonText">Text of the OK button</param>
        /// <returns></returns>
        PopupMenu MakeInfoPopup(string message, string continueButtonText);
        /// <summary>
        /// Registers a message that you want to appear when the game loads. This must be run in Start()!
        /// </summary>
        /// <param name="message">Message that appears when the game loads</param>
        void RegisterStartupPopup(string message);
    }
}
