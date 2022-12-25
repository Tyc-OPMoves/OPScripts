using System;
using UnityEngine;

public class MainInput : MonoBehaviour
{
    public static Action<float, float> _OnWASDInput;
    public static Action<int> _OnClickInput;
    public static Action<bool> _OnShiftInput;
    // subbed methods will check for int if user presses 1 to 6, or string checks if any single char key.
    public static Action<string, int> _OnANYInput;

    float _mainFireRate;
    float _mainFireRateTimer;
    float _secondaryFireRate;
    float _secondaryFireRateTimer;
    int _numberInput;
    string _letterInput;

    void OnEnable()
    {
        PlayerManager._OnFireRateUpdate += UpdatePlayerFireRate; //this is an event that you need to create in your PlayerManager script to get it's main and secondary fire rate 
    }
    private void OnDisable()
    {
        PlayerManager._OnFireRateUpdate -= UpdatePlayerFireRate;
    }

    void Update()
    {
        HOLDInputManager();
        DOWNInputManager();
    }

    void HOLDInputManager()
    {
        if (Input.anyKey) //so user can hold input for movement and fire. Add here any key that you would like to have the capability to be held.
        {
            float horizontalAxes = Input.GetAxis("Horizontal");
            float verticalAxes = Input.GetAxis("Vertical");

            bool leftClickInput = Input.GetMouseButton(0);
            bool rightClickInput = Input.GetMouseButton(1);

            bool ESCInput = Input.GetKeyDown(KeyCode.Escape);
            bool leftSHIFTDownInput = Input.GetKeyDown(KeyCode.LeftShift);

            if (horizontalAxes != 0 || verticalAxes != 0)
            {
                //Debug.Log("Invoking X: " + horizontalAxes + ", Y " + verticalAxes + " .");
                Debug.Log("User is pressing WASD");
                _OnWASDInput?.Invoke(horizontalAxes, verticalAxes);
            }

            //FireRate timer
            if (_mainFireRate > 0) //wait for player fire rate to initialize
            {

                if (leftClickInput && Time.time > _mainFireRateTimer)
                {
                    _mainFireRateTimer = Time.time + _mainFireRate;
                    Debug.Log("User Pressed: Left Click");
                    _OnClickInput?.Invoke(0);
                }
                if (rightClickInput && Time.time > _secondaryFireRateTimer)
                {
                    _secondaryFireRateTimer = Time.time + _secondaryFireRate;
                    Debug.Log("User Pressed: Right Click");
                    _OnClickInput?.Invoke(1);
                }
            }

            if (ESCInput)
            {
                Debug.Log("User Pressed: Escape");
                _OnANYInput?.Invoke("Esc", -1);
            }
            if (leftSHIFTDownInput)
            {
                _OnShiftInput?.Invoke(true);
            }
        }

        bool leftSHIFTUpInput = Input.GetKeyUp(KeyCode.LeftShift);
        if (leftSHIFTUpInput)
        {
            _OnShiftInput?.Invoke(false);
        }
    }
    //String-like keys (EX: Escape) need to be defined above.
    //Any single character on the keyboard is invoked below.
    //Subscribe to the event and check for lower-case or upper-case letters as desired.
    //In this game, this functionality is needed as holding "Shift" will result in an "Empowered" attack. 
    //If you do not care about lower/upper-case, the subscribed method can check for both the lower and upper case version of the key you desire.
    //WASD defined separately with individual Actions for ease of use with various methods that require specific calibration of the float values in the Axis. 
    //Mouse Clicks have int values to check for. 

    void DOWNInputManager()
    {
        if (Input.anyKeyDown) //so that "spells/abbilities" will get instantiated once per keypress
        {
            var GeneralInput = Input.inputString;

            //try to parse user input to a int and if it fails, we just cache the string.
            try
            {
                _numberInput = int.Parse(GeneralInput);
            }
            catch
            {
                _letterInput = GeneralInput;
            }

            if (_letterInput != null)
            {
                if (_letterInput.Length > 0)
                {
                    Debug.Log("User Pressed:" + _letterInput.ToString());
                    _OnANYInput?.Invoke(_letterInput, -1);
                    _letterInput = null; //to act as a "Down" type input. If you want "Sticky Keys" that togle, you can set a bool here.
                }
            }
            //below is just specific to this game where user will ever have 6 specific slots and I do not want 7 to 0 on the keyboard to ever be an input.
            //care that if Shift is used, below trasnform into string chars like "!,@,#,$,%" etc, so checks need to be done accordingly.
            if (_numberInput > 0 && _numberInput < 7)
            {
                Debug.Log("User Pressed: " + _numberInput);
                _OnANYInput?.Invoke(null, _numberInput);
                _numberInput = -1;
            }
        }
    }
    void UpdatePlayerFireRate(float mainFireRate, float secondaryFireRate)
    {
        _mainFireRate = mainFireRate;
        _secondaryFireRate = secondaryFireRate;
        Debug.Log("Updated Player stats to: mainFireRate: |" + mainFireRate + "| and secondaryFireRate: |" + secondaryFireRate + "|.");
    }

}
