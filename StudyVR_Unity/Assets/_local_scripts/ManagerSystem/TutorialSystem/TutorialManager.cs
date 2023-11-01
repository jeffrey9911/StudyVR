using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class TutorialManager : MonoBehaviour
{
    private Vector3 LeftAnchorPosition;
    private Vector3 RightAnchorPosition;
    private Quaternion LeftAnchorRotation;
    private Quaternion RightAnchorRotation;

    [SerializeField] private Transform LeftHandTrans;
    [SerializeField] private Transform RightHandTrans;
    [SerializeField] private Transform CentreEyeTrans;

    [SerializeField] private Transform LeftHandUI;
    [SerializeField] private Transform RightHandUI;
    [SerializeField] private GameObject TutorialUICanvas;
    [SerializeField] private TMP_Text TutorialText;

    [SerializeField] private GameObject TutorialButton1;
    [SerializeField] private GameObject TutorialButton2;
    [SerializeField] private GameObject TutorialButton3;
    [SerializeField] private GameObject TutorialButton4;
    [SerializeField] private GameObject TutorialButton5;

    private float FollowSpeed = 5.0f;

    [SerializeField] private GameObject L_GripTrigger;
    [SerializeField] private GameObject L_Joystick;
    [SerializeField] private GameObject L_Oculus;

    [SerializeField] private GameObject R_GripTrigger;
    [SerializeField] private GameObject R_HandTrigger;
    [SerializeField] private GameObject R_Joystick;
    [SerializeField] private GameObject R_Oculus;

    private int TutorialStep = -1;

    private float TutorialCounterUniversal = 0f;


    [SerializeField] private Transform FollowTransform;
    [SerializeField] private GameObject MovePanel;
    [SerializeField] private GameObject ScalePanel;

    private Vector3 LControllerPosRecord = new Vector3(0f, 0f, 0f);
    private Vector3 RControllerPosRecord = new Vector3(0f, 0f, 0f);

    private bool IsControllerPosRecorded = false;
    private Vector3 FollowUIScale = new Vector3(0.005f, 0.005f, 0.005f);

    private void Start()
    {
        TutorialUICanvas.SetActive(false);
        LeftHandUI.gameObject.SetActive(false);
        RightHandUI.gameObject.SetActive(false);
    }

    private void Update()
    {
        if (TutorialStep > 0)
        {
            Vector3 LeftPosAnchor = new Vector3(LeftHandTrans.position.x - CentreEyeTrans.position.x, 0f, LeftHandTrans.position.z - CentreEyeTrans.position.z);
            LeftAnchorPosition = LeftHandTrans.position + LeftPosAnchor.normalized * 0.2f + new Vector3(0f, 0.1f, 0f);

            Vector3 RightPosAnchor = new Vector3(RightHandTrans.position.x - CentreEyeTrans.position.x, 0f, RightHandTrans.position.z - CentreEyeTrans.position.z);
            RightAnchorPosition = RightHandTrans.position + RightPosAnchor.normalized * 0.2f + new Vector3(0f, 0.1f, 0f);

            LeftAnchorRotation = LeftHandTrans.rotation * Quaternion.Euler(30f, 0f, 0f);
            RightAnchorRotation = RightHandTrans.rotation * Quaternion.Euler(30f, 0f, 0f);

            LeftHandUI.position = Vector3.Lerp(LeftHandUI.position, LeftAnchorPosition, Time.deltaTime * FollowSpeed);
            RightHandUI.position = Vector3.Lerp(RightHandUI.position, RightAnchorPosition, Time.deltaTime * FollowSpeed);

            LeftHandUI.rotation = Quaternion.Lerp(LeftHandUI.rotation, LeftAnchorRotation, Time.deltaTime * FollowSpeed);
            RightHandUI.rotation = Quaternion.Lerp(RightHandUI.rotation, RightAnchorRotation, Time.deltaTime * FollowSpeed);

            TutorialUIFollow();

            switch (TutorialStep)
            {
                case 1:
                    if (TutorialCounterUniversal >= 4.0f)
                    {
                        TriggerStep2();
                    }
                    break;

                case 2:
                    if (OVRInput.Get(OVRInput.Axis2D.PrimaryThumbstick).magnitude > 0.1f)
                    {
                        TutorialCounterUniversal += Time.deltaTime;
                    }

                    if (TutorialCounterUniversal >= 2.0f)
                    {
                        TriggerStep3();
                    }
                    break;

                case 3:
                    if (OVRInput.Get(OVRInput.Axis2D.SecondaryThumbstick).magnitude > 0.1f)
                    {
                        TutorialCounterUniversal += Time.deltaTime;
                    }

                    if (TutorialCounterUniversal >= 2.0f)
                    {
                        TriggerStep4();
                    }
                    break;

                case 5:
                    if (TutorialCounterUniversal >= 2.0f)
                    {
                        TriggerStep6();
                    }
                    break;

                case 6:
                    if (TutorialCounterUniversal >= 2.0f)
                    {
                        TriggerStep7();
                    }
                    break;

                case 7:
                    if (TutorialCounterUniversal >= 2.0f)
                    {
                        TriggerStepEnd();
                    }
                    break;

                default:
                    break;
            }
        }

    }

    public void StartTutorial()
    {
        FollowTransform.localPosition = new Vector3(0f, 0.268f, 1.171f);
        TutorialUICanvas.SetActive(true);
        LeftHandUI.gameObject.SetActive(true);
        RightHandUI.gameObject.SetActive(true);
        TriggerStep1();
    }

    private void TriggerStep1() // Right hand trigger: Select and Click
    {
        SwitchOff();
        R_HandTrigger.SetActive(true);

        TutorialButton1.SetActive(true);
        TutorialButton2.SetActive(true);
        TutorialButton3.SetActive(true);
        TutorialButton4.SetActive(true);
        TutorialButton5.SetActive(false);

        TutorialText.text = "Welcome to the tutorial!\nNow practise\nblue laser and right hand trigger\nto aim and click";

        TutorialCounterUniversal = 0.0f;

        TutorialStep = 1;
    }

    private void TriggerStep2() // Left joystick: Move
    {
        SwitchOff();
        L_Joystick.SetActive(true);

        TutorialButton1.SetActive(false);
        TutorialButton2.SetActive(false);
        TutorialButton3.SetActive(false);
        TutorialButton4.SetActive(false);
        TutorialButton5.SetActive(false);

        TutorialText.text = "Use\nleft joystick\nto move around";

        TutorialCounterUniversal = 0.0f;

        TutorialStep = 2;
    }

    private void TriggerStep3() // Right joystick: Rotate
    {
        SwitchOff();
        R_Joystick.SetActive(true);

        TutorialButton1.SetActive(false);
        TutorialButton2.SetActive(false);
        TutorialButton3.SetActive(false);
        TutorialButton4.SetActive(false);
        TutorialButton5.SetActive(false);

        TutorialText.text = "Use\nright joystick\nto rotate yourself";

        TutorialCounterUniversal = 0.0f;

        TutorialStep = 3;
    }

    private void TriggerStep4() // Right oculus: Recentre
    {
        SwitchOff();
        R_Oculus.SetActive(true);

        TutorialButton1.SetActive(false);
        TutorialButton2.SetActive(false);
        TutorialButton3.SetActive(false);
        TutorialButton4.SetActive(false);
        TutorialButton5.SetActive(true);

        TutorialText.text = "Press down & hold\nright OCULUS button\nto recenter yourself";

        TutorialCounterUniversal = 0.0f;

        TutorialStep = 4;
    }

    private void TriggerStep5() // Left oculus: Hide/Display
    {
        SwitchOff();
        L_Oculus.SetActive(true);

        TutorialButton1.SetActive(false);
        TutorialButton2.SetActive(false);
        TutorialButton3.SetActive(false);
        TutorialButton4.SetActive(false);
        TutorialButton5.SetActive(false);

        TutorialText.text = "Click\nleft menu button\nto Hide/Display the panel";

        TutorialCounterUniversal = 0.0f;

        TutorialStep = 5;
    }

    private void TriggerStep6() // Left or right grip trigger: Move UI
    {
        SwitchOff();
        L_GripTrigger.SetActive(true);
        R_GripTrigger.SetActive(true);

        TutorialButton1.SetActive(false);
        TutorialButton2.SetActive(false);
        TutorialButton3.SetActive(false);
        TutorialButton4.SetActive(false);
        TutorialButton5.SetActive(false);

        TutorialText.text = "Grip\nleft OR right grip trigger\nto move the panel";

        TutorialCounterUniversal = 0.0f;

        TutorialStep = 6;
    }

    private void TriggerStep7() // Left and right grip trigger: Scale UI
    {
        SwitchOff();
        L_GripTrigger.SetActive(true);
        R_GripTrigger.SetActive(true);

        TutorialButton1.SetActive(false);
        TutorialButton2.SetActive(false);
        TutorialButton3.SetActive(false);
        TutorialButton4.SetActive(false);
        TutorialButton5.SetActive(false);

        TutorialText.text = "Grip\nleft AND right grip trigger\nto scale the panel";

        TutorialCounterUniversal = 0.0f;

        TutorialStep = 7;
    }

    private void TriggerStepEnd()
    {
        TutorialStep = -1;
        SwitchOff();
        TutorialUICanvas.SetActive(false);
    }

    void SwitchOff()
    {
        L_GripTrigger.SetActive(false);
        L_Joystick.SetActive(false);
        L_Oculus.SetActive(false);

        R_GripTrigger.SetActive(false);
        R_HandTrigger.SetActive(false);
        R_Joystick.SetActive(false);
        R_Oculus.SetActive(false);
    }

    void TutorialUIFollow()
    {
        if (OVRInput.GetDown(OVRInput.Button.Start) && TutorialStep >= 5)
        {
            TutorialUICanvas.SetActive(!TutorialUICanvas.activeSelf);
            if (TutorialStep == 5)
            {
                TutorialCounterUniversal += 1.0f;
            }
        }

        bool isLeftHandTrigger = OVRInput.Get(OVRInput.Button.PrimaryHandTrigger);
        bool isRightHandTrigger = OVRInput.Get(OVRInput.Button.SecondaryHandTrigger);

        if ((isLeftHandTrigger || isRightHandTrigger) && TutorialStep >= 6)
        {
            if (TutorialStep == 6) TutorialCounterUniversal += Time.deltaTime;

            MovePanel.SetActive(true);
            ScalePanel.SetActive(false);

            if (!IsControllerPosRecorded)
            {
                LControllerPosRecord = LeftHandTrans.position;
                RControllerPosRecord = RightHandTrans.position;
                IsControllerPosRecorded = true;
            }

            if (isLeftHandTrigger && isRightHandTrigger && TutorialStep >= 7)
            {
                if (TutorialStep == 7) TutorialCounterUniversal += Time.deltaTime;

                MovePanel.SetActive(false);
                ScalePanel.SetActive(true);

                float dDistance = Vector3.Distance(LeftHandTrans.position, RightHandTrans.position) - Vector3.Distance(LControllerPosRecord, RControllerPosRecord);
                dDistance *= 0.006f;

                FollowUIScale += new Vector3(dDistance, dDistance, dDistance);
            }
            else if (isLeftHandTrigger)
            {
                FollowTransform.position += LeftHandTrans.position - LControllerPosRecord;
            }
            else if (isRightHandTrigger)
            {
                FollowTransform.position += RightHandTrans.position - RControllerPosRecord;
            }

            LControllerPosRecord = LeftHandTrans.position;
            RControllerPosRecord = RightHandTrans.position;
        }
        else
        {
            MovePanel.SetActive(false);
            ScalePanel.SetActive(false);
            IsControllerPosRecorded = false;
        }

        Quaternion lookatRot = Quaternion.LookRotation(CentreEyeTrans.position - FollowTransform.position, Vector3.up);
        lookatRot *= Quaternion.Euler(0f, 180f, 0f);
        FollowTransform.rotation = lookatRot;

        TutorialUICanvas.transform.position = Vector3.Lerp(TutorialUICanvas.transform.position, FollowTransform.position, Time.deltaTime * FollowSpeed);
        TutorialUICanvas.transform.rotation = Quaternion.Lerp(TutorialUICanvas.transform.rotation, FollowTransform.rotation, Time.deltaTime * FollowSpeed);
        TutorialUICanvas.transform.localScale = Vector3.Lerp(TutorialUICanvas.transform.localScale, FollowUIScale, Time.deltaTime * FollowSpeed);


    }

    public void TutorialButtonOnClick(Button button)
    {
        TutorialCounterUniversal += 1.0f;

        if(button != null)
        {
            button.gameObject.SetActive(false);
        }
    }

    public void RecenteredButtonOnClick()
    {
        TriggerStep5();
    }


    [ContextMenu("DebugNextStep")]
    public void DebugNextStep()
    {
        switch (TutorialStep)
        {
            case 1:
                TriggerStep2();
                break;

            case 2:
                TriggerStep3();
                break;

            case 3:
                TriggerStep4();
                break;

            case 4:
                TriggerStep5();
                break;

            case 5:
                TriggerStep6();
                break;

            case 6:
                TriggerStep7();
                break;

            case 7:
                TriggerStepEnd();
                break;

            default:
                break;
        }
    }
}
