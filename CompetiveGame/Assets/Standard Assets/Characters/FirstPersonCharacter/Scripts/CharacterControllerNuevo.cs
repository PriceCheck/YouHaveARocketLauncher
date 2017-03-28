using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using XInputDotNetPure; // Required in C#

public class CharacterControllerNuevo : MonoBehaviour {

    //Input Controller
    bool playerIndexSet = false;
    PlayerIndex playerIndex;
    GamePadState state;
    GamePadState prevState;
    //
    public GameObject RotationTarget;
    float rotationSpeed = 90;
    float MoveSpeed = 17;
    float Gravity = -9.8f;
    public AnimationCurve LookRamp = AnimationCurve.EaseInOut(-1, 0, 1, 1);
    public AnimationCurve MoveRamp = AnimationCurve.EaseInOut(-1, 0, 1, 1);


    public void OnCollide()
    {

    }
	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
        // Find a PlayerIndex, for a single player game
        // Will find the first controller that is connected ans use it
        if (!playerIndexSet || !prevState.IsConnected)
        {
            for (int i = 0; i < 4; ++i)
            {
                PlayerIndex testPlayerIndex = (PlayerIndex)i;
                GamePadState testState = GamePad.GetState(testPlayerIndex);
                if (testState.IsConnected)
                {
                   // Debug.Log(string.Format("GamePad found {0}", testPlayerIndex));
                    playerIndex = testPlayerIndex;
                    playerIndexSet = true;
                }
            }
        }
        prevState = state;
        state = GamePad.GetState(playerIndex);

        Vector2 MoveVector = new Vector2(state.ThumbSticks.Left.X, state.ThumbSticks.Left.Y);
        MoveVector = MoveRamp.Evaluate(MoveVector.magnitude) * MoveVector.normalized;
        Vector2 LookVector = new Vector2(-state.ThumbSticks.Right.Y, state.ThumbSticks.Right.X);
        LookVector = LookRamp.Evaluate(LookVector.magnitude) * LookVector.normalized;
      //  print(MoveVector);
 
        // *= Quaternion.Euler(, , 0);
        Vector3 newAngle = RotationTarget.transform.rotation.eulerAngles + new Vector3(LookVector.x * rotationSpeed * Time.deltaTime, LookVector.y * rotationSpeed * Time.deltaTime, 0);//LookVector.y * rotationSpeed * Time.deltaTime);
        RotationTarget.transform.rotation = Quaternion.Euler(newAngle);

        Vector3 DeltaMove = Quaternion.Euler(new Vector3(0, newAngle.y, 0)) * new Vector3(MoveVector.x * MoveSpeed * Time.deltaTime, 0, MoveVector.y * MoveSpeed * Time.deltaTime);
        DeltaMove += new Vector3(0, Gravity * Time.deltaTime, 0);
        //Raycast for obsticles 
        Ray direction = new Ray(transform.position, transform.position + DeltaMove);
        //  print(DeltaMove);
        //  print(transform.position + "," + (transform
        // print(hit.transform.gameObject);
        // DeltaMove = hit.distance * DeltaMove;

        GetComponent<CharacterController>().Move(DeltaMove);
    }
}
