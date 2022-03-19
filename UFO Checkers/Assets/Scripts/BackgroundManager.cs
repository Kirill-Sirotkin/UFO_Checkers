using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace UFOCheckers
{
    public class BackgroundManager : MonoBehaviour
    {
        [field: SerializeField] private GameObject BackgroundParentObject;

        [field: SerializeField] private GameObject midday_objects;
        [field: SerializeField] private GameObject evening_objects;
        [field: SerializeField] private GameObject midnight_objects;
        [field: SerializeField] private GameObject sunset_objects;
        [field: SerializeField] private GameObject daybreak_objects;

        private Vector2[] starting_points = new Vector2[] { new Vector2(-400f, 0f), new Vector2(0f, 0f), new Vector2(400f, 0f) };

        private GameObject used_background_spawn_objects;
        private GameObject[] background;

        private int counter = 0;

        [field: SerializeField] private float speed = 5f;
        private float direction = 1f;
        private float limit = 400f;

        private void Awake()
        {
            //DontDestroyOnLoad(gameObject);
            background = new GameObject[starting_points.GetLength(0)];
            DetermineBackground();
            InitializeBackground();
            DetermineDirection();
        }

        private void Update()
        {
            MoveBackground(Time.deltaTime);
            CheckBackgroundLimit();
        }

        private void DetermineBackground() 
        {
            int rand = Random.Range(1, 6);
            //print("bg: " + rand);

            switch (rand) 
            {
                case 1:
                    used_background_spawn_objects = midday_objects;
                    break;
                case 2:
                    used_background_spawn_objects = evening_objects;
                    break;
                case 3:
                    used_background_spawn_objects = midnight_objects;
                    break;
                case 4:
                    used_background_spawn_objects = sunset_objects;
                    break;
                case 5:
                    used_background_spawn_objects = daybreak_objects;
                    break;
            }
        }

        private void DetermineDirection()
        {
            int rand = Random.Range(1, 3);
            //print("dir: " + rand);

            switch (rand) 
            {
                case 1:
                    break;
                case 2:
                    direction *= -1f;
                    limit *= -1f;
                    break;
            }
        }

        private void InitializeBackground() 
        {
            for (int i = 0; i < starting_points.GetLength(0); i++) 
            {
                background[i] = Instantiate(used_background_spawn_objects, starting_points[i], Quaternion.identity, BackgroundParentObject.transform);
            }
        }

        private void MoveBackground(float time)
        {
            for (int i = 0; i < background.GetLength(0); i++)
            {
                background[i].transform.position = background[i].transform.position + (new Vector3(direction, 0f, 0f) * speed * time);
            }
        }

        private void CheckBackgroundLimit()
        {
            for (int i = 0; i < background.GetLength(0); i++)
            {
                if (limit < 0f)
                {
                    if (background[i].transform.position.x < limit) 
                    {
                        background[i].transform.position = new Vector3(background[counter].transform.position.x + 400f, 0f, 0f);
                        ModifyCounter(1);
                    }
                }
                if (limit > 0f)
                {
                    if (background[i].transform.position.x > limit)
                    {
                        background[i].transform.position = new Vector3(background[counter].transform.position.x - 400f, 0f, 0f);
                        ModifyCounter(-1);
                    }
                }
            }
        }

        private void ModifyCounter(int value) 
        {
            counter += value;

            if(counter < 0) 
            {
                counter = background.GetLength(0) - 1;
            }
            if (counter > (background.GetLength(0) - 1))
            {
                counter = 0;
            }
        }
    }
}
