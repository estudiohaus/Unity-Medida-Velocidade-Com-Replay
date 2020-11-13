using System;
using System.Linq;
using System.Collections.Generic;
using UnityEngine;
using ChartAndGraph;
using TMPro;
using EstudioHaus.Plugins.SpeedMeasurement.Models;

public class ExampleResultView : MonoBehaviour
{
    #region Inspector Fields
    public TextMeshProUGUI averageSpeedField, maximumSpeedField, timeField, approvedField;
    public GraphChart measureChart;
    public string categoryName = "Average Speed";
    #endregion

    public void GenerateResult(List<MeasurePoint> measurePoints)
    {
        double pathLength = measurePoints[measurePoints.Count - 1].Meter;
        double totalTime = measurePoints[measurePoints.Count - 1].Second;
        double finalAverageSpeed = pathLength / totalTime;

        averageSpeedField.text = $"{Math.Round(finalAverageSpeed, 2)} m/s";
        maximumSpeedField.text = $"{Math.Round(measurePoints.Max(m => m.AverageSpeed), 2)} m/s";
        timeField.text = $"{Math.Round(totalTime, 0)} s";
        approvedField.text = measurePoints.Any(m => m.AverageSpeed > 2) ? "Não" : "Sim";

        GenerateMeasureChart(measurePoints);
    }

    private void GenerateMeasureChart(List<MeasurePoint> measurePoints)
    {
        measureChart.DataSource.StartBatch();
        measureChart.DataSource.ClearCategory(categoryName);

        foreach (MeasurePoint point in measurePoints)
            measureChart.DataSource.AddPointToCategory(categoryName, point.Second, point.AverageSpeed);

        measureChart.DataSource.EndBatch();
    }
}