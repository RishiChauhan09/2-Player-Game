using System.Collections;
using UnityEngine;

public class Board_Rotation : MonoBehaviour
{
    [System.Serializable]
    public class RotationElements
    {
        #pragma warning disable 0649
        public float Speed;
        public float Duration;
        #pragma warning restore 0649
    }

    [SerializeField]
    private RotationElements[] rotationElements;

    private WheelJoint2D WheelJoint;
    private JointMotor2D Motor;

    private void Awake()
    {
        WheelJoint = GetComponent<WheelJoint2D>();
        Motor = new JointMotor2D();
        StartCoroutine(Rotate());
    }

    private IEnumerator Rotate()
    {
        int RoatationIndex = 0;
        while (true)
        {
            yield return new WaitForFixedUpdate();

            Motor.motorSpeed = rotationElements[RoatationIndex].Speed;
            Motor.maxMotorTorque = 10000;
            WheelJoint.motor = Motor;

            yield return new WaitForSecondsRealtime(rotationElements[RoatationIndex].Duration);
            RoatationIndex++;
            RoatationIndex = RoatationIndex < rotationElements.Length ? RoatationIndex : 0;

        }
    }
}
