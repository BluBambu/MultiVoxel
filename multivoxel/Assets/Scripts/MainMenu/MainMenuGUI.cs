using System;
using UnityEngine;
using UnityEngine.UI;
using UnityEngineInternal;

public class MainMenuGUI : MonoBehaviour
{
    public Button ExitButton;
    public Button ServerButton;
    public Button ClientButton;
    public InputField IpInputField;
    public InputField PortInputField;
    public InputField LoadInputField;
    public Text ErrorTextBox;

    // Has the side effect of showing the user the message of a
    // malformed port if what's inside the port field isn't an
    // integer when this property is 
    //
    // Also returns -1 if the port isn't an integer
    private int Port
    {
        get
        {
            int port;
            if (!Int32.TryParse(PortInputField.text, out port))
            {
                WriteToErrorBox("Port must be a number.");
                return -1;
            }
            return port;
        }
    }

    private string IpAddress { get { return IpInputField.text; } }

    private string LoadPath { get { return LoadInputField.text; } }

    private void Awake()
    {
        ServerButton.onClick.AddListener(() =>
        {
            // Replace with successful server creation condition
			Server.Start(Port, Config.SERVER_LOG_FILE);
        });

        ClientButton.onClick.AddListener(() =>
        {
            // Replace with successful client creation condition
			Client.Start(IpAddress, Port, Config.CLIENT_LOG_FILE);
            if (true)
            {
                Application.LoadLevel(1);
            }
        });

        ExitButton.onClick.AddListener(Application.Quit);
    }

    private void WriteToErrorBox(string msg)
    {
        ErrorTextBox.text = msg;
    }
}
