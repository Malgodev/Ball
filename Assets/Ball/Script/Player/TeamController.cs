using System.Collections;
using System.Collections.Generic;
using System.Drawing;
using UnityEngine;

namespace TeamController
{
    class FormationRectangle
    {
        // center X, Y allow team to move for defense and attack option
        public float centerX { get; private set; }
        public float centerY { get; private set; }

        // width, height allow team to compress into a smaller area for more aggresive play and vice versa
        public float width { get; private set; }
        public float height { get; private set; }

       public FormationRectangle(float centerX, float centerY, float width, float height)
        {
            this.centerX = centerX;
            this.centerY = centerY;
            this.width = width;
            this.height = height;
        }
    }

    public class TeamController : MonoBehaviour
    {
        [SerializeField] Formation formation;

        [SerializeField] FormationRectangle formationMovement;

        [SerializeField] GameObject PlayerPrefab;

        [SerializeField] GameObject Square;

        private void Awake()
        {
            formationMovement = new FormationRectangle(-15f, 0f, 40f, 30f);

#if UNITY_EDITOR
            Square.SetActive(true);
#endif
        }

        void Start()
        {

            switch (formation)
            {

               case Formation.Formation_3_3_2:
                
                    break;

                default:
                    Debug.Log("Not choose formation yet");
                    break;
            }
        }

        // Update is called once per frame
        void Update()
        {
#if UNITY_EDITOR
            Square.transform.position = new Vector3(formationMovement.centerX, formationMovement.centerY, 0f);
            Square.transform.localScale = new Vector3(formationMovement.width, formationMovement.height, 1f);
#endif
        }
    }
}
