using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace EasyGameStudio.Jeremy
{
    public class Demo_control : MonoBehaviour
    {

        public AudioSource audio_source;
        public AudioClip ka;

        public GameObject[] game_object_walls;
        public GameObject[] game_object_robots;


        private int index_walls = 0;
        private int index_robot = 0;



        void Start()
        {
            this.index_walls = 0;
            this.index_robot = 0;
        }



        public void on_previous_btn(int num)
        {

            if (num == 0)
            {
                this.index_walls--;
                if (this.index_walls <= -1)
                    this.index_walls = (this.game_object_walls.Length - 1);


                for (int i = 0; i < this.game_object_walls.Length; i++)
                {
                    if (i == this.index_walls)
                    {
                        this.game_object_walls[i].SetActive(true);
                    }
                    else
                    {
                        this.game_object_walls[i].SetActive(false);
                    }
                }
            }
            else if (num == 1)
            {
                this.index_robot--;
                if (this.index_robot <= -1)
                    this.index_robot = (this.game_object_robots.Length - 1);

                for (int i = 0; i < this.game_object_robots.Length; i++)
                {
                    if (i == this.index_robot)
                    {
                        this.game_object_robots[i].SetActive(true);
                    }
                    else
                    {
                        this.game_object_robots[i].SetActive(false);
                    }
                }
            }




            this.audio_source.PlayOneShot(this.ka);
        }

        public void on_next_btn(int num)
        {
            if (num == 0)
            {

                this.index_walls++;
                if (this.index_walls >= this.game_object_walls.Length)
                    this.index_walls = 0;

                for (int i = 0; i < this.game_object_walls.Length; i++)
                {
                    if (i == this.index_walls)
                    {
                        this.game_object_walls[i].SetActive(true);
                    }
                    else
                    {
                        this.game_object_walls[i].SetActive(false);
                    }
                }
            }
            else if (num == 1)
            {
                this.index_robot++;
                if (this.index_robot >= this.game_object_robots.Length)
                    this.index_robot = 0;

                for (int i = 0; i < this.game_object_robots.Length; i++)
                {
                    if (i == this.index_robot)
                    {
                        this.game_object_robots[i].SetActive(true);
                    }
                    else
                    {
                        this.game_object_robots[i].SetActive(false);
                    }
                }
            }
            this.audio_source.PlayOneShot(this.ka);
        }
    }
}
