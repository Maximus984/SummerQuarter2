using System.Runtime.CompilerServices;
using UnityEngine;

public class GroundTile : MonoBehaviour
{
   // controls the speed of the ground tiles

   [SerializeField] private float moveSpeed = 2f;

    private void FixedUpdate()
    {
        if( GameManager.isGameOver) return; // End this function early if the game is over
        
        //move the groud tile towards the player 
        transform.position += new UnityEngine.Vector3(0, 0, -moveSpeed) * Time.deltaTime; 
    } 
}
