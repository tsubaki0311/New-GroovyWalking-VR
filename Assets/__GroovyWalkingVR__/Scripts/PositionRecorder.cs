using System;
using System.Text;
using System.IO;
using UnityEngine;
using System.Collections.Generic;

public class PositionRecorder : MonoBehaviour
{
    List<Vector3> _targetPositions = new List<Vector3>();
    StreamWriter _streamWriter;
    string _filePath;

    private void Awake()
    {
        _filePath = Application.dataPath + @"\csv\" + DateTime.Now.ToString("yyyy-MM-dd_hh-mm-ss-mm") + ".csv";
    }

    public void RecordOnePos(Vector3 pos)
    {
        _targetPositions.Add(pos);
    }

    private void OnDestroy()
    {
        using(StreamWriter sw = new StreamWriter(_filePath, false, Encoding.UTF8))
        {
            string[] header = { "posX", "posY", "posZ" };
            string headerAsCsvRow = string.Join(",", header);
            sw.WriteLine(headerAsCsvRow);

            foreach (var eachPos in _targetPositions)
            {
                string[] points = { eachPos.x.ToString(), eachPos.y.ToString(), eachPos.z.ToString() };
                string pointAsCsvRow = string.Join(",", points);
                sw.WriteLine(pointAsCsvRow);
            }
        }

        Debug.Log(_filePath);
    }
}
