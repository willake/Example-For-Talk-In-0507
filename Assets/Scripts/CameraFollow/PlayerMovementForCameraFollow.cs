﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMovementForCameraFollow : MonoBehaviour 
{

	// Player Rigibody reference
	Rigidbody2D playerRig;

	// Speed for player movement
	public float MoveSpeed;

	[HeaderAttribute("Player Status")]
	public bool facingRight = true;
	public bool movingUp = false;

	[HeaderAttribute("Ground Cast")]
	public float preCastDistance;
	public float castOffset;
	public float castDistance;

	// Player Position Reference
	public GameObjectPositionVariable playerPosition;

	void Start()
	{
		playerRig = GetComponent<Rigidbody2D>();
	}

	// Update is called once per frame
	void Update () 
	{
		// If player click A or D
		if(Input.GetKey(KeyCode.A)||Input.GetKey(KeyCode.D))
		{
			Move();
		}


		// Save position to player position assets, you don't really need to know about it
		SetPositionToAssets();
	}

	void Move()
	{
		// InputX is work with Unity, if player push A or D button on your keyboard it will change,   A = 0   D = -1
		float inputX = Input.GetAxis("Horizontal");

		// Player facing direction, flipX is the direction of the picture
		facingRight = inputX>0?true:false;
		GetComponent<SpriteRenderer>().flipX = facingRight?false:true;

		// Init vector
		Vector2 moveVector = Vector2.zero;
		
		moveVector = GroundVectorCast();
		moveVector.x *= inputX * MoveSpeed;
		moveVector.y *= (movingUp? 1 : -1) * MoveSpeed;
		if(moveVector == Vector2.zero)
		{
			moveVector = new Vector2(inputX*MoveSpeed,0);
		}

		playerRig.velocity = moveVector;

		// You may need to know about Debug.Log
		Debug.Log(inputX);
	}

	Vector2 GroundVectorCast()
	{
		RaycastHit2D hitLeft = Physics2D.Raycast(transform.position + new Vector3(castOffset + (facingRight?preCastDistance:-preCastDistance),0,0), -Vector2.up,castDistance,1 << LayerMask.NameToLayer("Ground"));
		RaycastHit2D hitRight = Physics2D.Raycast(transform.position + new Vector3(-castOffset + (facingRight?preCastDistance:-preCastDistance),0,0), -Vector2.up,castDistance,1 << LayerMask.NameToLayer("Ground"));

		if(hitLeft)
		{
			Debug.DrawLine(transform.position + new Vector3(castOffset + (facingRight?preCastDistance:-preCastDistance),0,0),hitLeft.point,Color.red);
		}

		if(hitRight)
		{
			Debug.DrawLine(transform.position + new Vector3(-castOffset + (facingRight?preCastDistance:-preCastDistance),0,0),hitRight.point,Color.red);
		}

		if(hitLeft&&hitRight)
		{
			float vectorX = Mathf.Abs(hitLeft.point.x - hitRight.point.x);
			float vectorY = hitLeft.point.y>hitRight.point.y ? Mathf.Abs(hitLeft.point.y - hitRight.point.y):Mathf.Abs(hitRight.point.y - hitLeft.point.y);
			
			if(facingRight)
			{
				movingUp = hitLeft.point.y>hitRight.point.y ? true : false;
			}
			else
			{
				movingUp = hitLeft.point.y>hitRight.point.y ? false : true;
			}

			return new Vector2(vectorX,vectorY);
		}
		

		return Vector2.zero;
	}

	void SetPositionToAssets(){
		playerPosition.value = transform.position;
	}
}
