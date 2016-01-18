﻿using Entitas;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

namespace Assets.LevelEditor
{
    public class UIInteractions : MonoBehaviour
    {
        public Text PositionInfo;

        private string _lastUsedPath = "";

        public void Start()
        {
            _lastUsedPath = FileOperations.FileOperations.GetLastUsedPath();
        }

        public void Save()
        {
            if (_lastUsedPath != "")
            {
                SaveAs(_lastUsedPath);
            }
        }

        public void SaveAs(string path)
        {
            _lastUsedPath = path;
            FileOperations.FileOperations.Save(path);
        }

        public void Load(string path)
        {
            Clear();
            _lastUsedPath = path;
            FileOperations.FileOperations.Load(path);
        }

        public void Clear()
        {
            _lastUsedPath = "";
            FileOperations.FileOperations.SetLastUsedPath("");
            Pools.pool.Clear(Matcher.AnyOf(Matcher.Tile, Matcher.Item));
            EditorSetup.Instance.Update();
        }

        public void Play()
        {
            if(_lastUsedPath != "")
            {
                SaveAs(_lastUsedPath);
                PlaySetup.FromEditor = true;
                SceneManager.LoadScene("Play");
            }
            else
            {
                Debug.Log("Can only play level if it has been saved to a file");
            }
        }

        public void Update()
        {
            if(Pools.pool.inputEntity.hasPosition)
            {
                var position = Pools.pool.inputEntity.position.Value;
                PositionInfo.text = string.Format("X: {0}\nZ: {1}", position.X, position.Z);
            }

            if (UnityEngine.Input.GetKeyDown(KeyCode.Return))
            {
                Play();
            }
        }
    }
}