﻿using UnityEngine;
using UnityEditor;
using UnityEngine.UIElements;

namespace UIViews
{
    /// <summary>
    /// Provides base functionality to PopUp windows.
    /// </summary>
    public abstract class PopUp : UIScript
    {
        /// <summary>
        /// The background colour of the PopUp's cover element.
        /// </summary>
        protected Color PopupBackgroundColor { get; set; } = new Color(55, 55, 55, 199);

        /// <summary>
        /// The base container on the Navigator's target document.
        /// </summary>
        private VisualElement BaseContainer;

        /// <summary>
        /// The PopUp window's content container. Load the actual window contents here.
        /// </summary>
        protected VisualElement ContentContainer { get; private set; }

        void Start()
        {
            if (Navigator != null)
            {
                ContainerID = "BasePopUpContainer";
                BaseContainer = Navigator.GetTargetContainer(ContainerID);
            }
            DisplayView();
        }

        public override void DisplayView()
        {
            if (BaseContainer != null)
            {
                VisualElement popupBase = BuildPopUpBaseElement();

                // OnEnterFocus
                popupBase.RegisterCallback<AttachToPanelEvent>(evt => {
                    OnEnterFocus(null, null);
                    IsViewActive = true;
                });

                // OnLeaveFocus
                popupBase.RegisterCallback<DetachFromPanelEvent>(evt => {
                    OnLeaveFocus(null, null);
                    IsViewActive = false;
                });

                BaseContainer.Clear();
                BaseContainer.Add(popupBase);
                OnEnterFocus(null, null);
            }
        }

        /// <summary>
        /// Builds and sets up the base PopUp window, with all events attached. This is done in code because reliably referencing a VisualTreeAsset in code is pain.
        /// </summary>
        /// <returns></returns>
        private VisualElement BuildPopUpBaseElement()
        {
            VisualElement popupBase = new VisualElement() 
            { 
                name = WrapperVisualElementName, 
                style = { 
                    position = Position.Absolute,
                    top = 0,
                    display = DisplayStyle.Flex,
                    overflow = Overflow.Visible,
                    justifyContent = Justify.Center,
                    width = Length.Percent(100),
                    height = Length.Percent(100),
                    backgroundColor = PopupBackgroundColor
                } 
            };
            VisualElement popupContentContainer = new VisualElement()
            {
                name = "PopUp_ContentContainer",
                style = {
                    flexGrow = 1,
                    alignItems = Align.Center,
                    justifyContent = Justify.Center
                }
            };
            Button popupCloseButton = new Button()
            {
                name = "PopUp_CloseButton",
                style = {
                    position = Position.Absolute,
                    top = 0,
                    right = 0,
                    width = 150,
                    height = 150,
                    backgroundColor = new Color(0, 0, 0, 0),
                    borderBottomWidth = 0,
                    borderLeftWidth = 0,
                    borderTopWidth = 0,
                    borderRightWidth = 0,
                    marginTop = 25,
                    marginBottom = 25,
                    marginLeft = 25,
                    marginRight = 25,
                    paddingLeft = 15,
                },
            };
            popupCloseButton.clicked += ClosePopUpWindow;

            UXMLDocument.CloneTree(popupContentContainer);

            popupBase.Add(popupContentContainer);
            popupBase.Add(popupCloseButton);

            return popupBase;
        }

        /// <summary>
        /// Closes the PopUp. Same as <see cref="UIScript.HideView()"/>.
        /// </summary>
        protected void ClosePopUpWindow()
        {
            HideView();
        }


        [CustomEditor(typeof(PopUp), true, isFallback = true)]
        public class PopUpEditor : Editor
        {
            private bool IsFoldoutOpen = true;

            public override void OnInspectorGUI()
            {
                // Get the view data
                var popup = target as PopUp;

                // View setup is in its own foldout so it doesn't take up too much space on derived scripts
                IsFoldoutOpen = EditorGUILayout.BeginFoldoutHeaderGroup(IsFoldoutOpen, "View setup");
                if (IsFoldoutOpen)
                {
                    // Increase ident by 1 so the foldout is more visually separated
                    EditorGUI.indentLevel++;

                    // Set the UI Navigator instance
                    popup.Navigator = (ViewNavigator)EditorGUILayout.ObjectField("UI Navigator", popup.Navigator, typeof(ViewNavigator), true);
                    
                    EditorGUILayout.Space(10);

                    popup.ID = EditorGUILayout.TextField("ID", popup.ID);
                    popup.UXMLDocument = (VisualTreeAsset)EditorGUILayout.ObjectField("UI Document", popup.UXMLDocument, typeof(VisualTreeAsset), true);
                    popup.IsStatic = EditorGUILayout.Toggle("Is Static", popup.IsStatic);
                    // Reset ident to normal
                    EditorGUI.indentLevel--;
                }

                EditorGUILayout.EndFoldoutHeaderGroup();
                EditorGUILayout.Space(10);

                // Draw the default inspector last so the script's UI can be separated. This will draw the auto UI as normal for derived scripts
                DrawDefaultInspector();
            }
        }
    }
}