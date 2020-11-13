using System;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using TMPro;
using Cinemachine;
using EstudioHaus.Plugins.SpeedMeasurement.Controllers;
using EstudioHaus.Plugins.SpeedMeasurement.Models;
using System.Collections;

public class ExampleController : MonoBehaviour
{
    #region Inspector Fields
    public SpeedMeasurer speedMeasurer;
    public ReplayController replayController;
    public ExampleResultView exampleResultView;
    [Space(10f)]
    public MovementInput playerMovementInput;
    public CinemachineFreeLook auxiliarCamera1;
    public CinemachineVirtualCamera auxiliarCamera2;
    public GameObject player, playerClone;
    [Space(10f)]
    public GameObject initialPanel;
    public GameObject pausePanel, informationPanel, speedometerContainer;
    public TextMeshProUGUI speedometer;
    public Slider speedLimitIndicator;
    #endregion

    private Vector3 cloneOriginalPosition;
    private bool isTheGamePaused = false;
    private bool isSliderChanging = false;
    private bool changeToUp = true;
    private int nextSliderValue;

    private void Start()
    {
        playerMovementInput.enabled = auxiliarCamera1.enabled = auxiliarCamera2.enabled = false;
        playerMovementInput.Velocity = 3f;
        cloneOriginalPosition = playerClone.transform.position;
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.LeftShift)) playerMovementInput.Velocity = 1.5f;
        if (Input.GetKeyUp(KeyCode.LeftShift)) playerMovementInput.Velocity = 3f;

        if (Input.GetKeyDown(KeyCode.Return)) ChangePauseStatus();

        if (Input.GetKeyDown(KeyCode.Space)) ChangeTimeVelocity(0.1f);
        if (Input.GetKeyUp(KeyCode.Space)) ChangeTimeVelocity(1);

        if (isSliderChanging)
        {
            if (changeToUp)
            {
                if (speedLimitIndicator.value >= nextSliderValue)
                {
                    isSliderChanging = false;
                    return;
                }
                speedLimitIndicator.value += Time.deltaTime * 2;
            }
            else
            {
                if (speedLimitIndicator.value < nextSliderValue)
                {
                    isSliderChanging = false;
                    return;
                }
                speedLimitIndicator.value -= Time.deltaTime * 2;
            }
        }
    }

    public void ChangePauseStatus()
    {
        if (initialPanel.activeSelf || informationPanel.activeSelf) return;

        isTheGamePaused = !isTheGamePaused;
        Time.timeScale = isTheGamePaused ? 0 : 1;
        pausePanel.SetActive(isTheGamePaused);
    }

    public void ChangeTimeVelocity(float timeScale)
    {
        if (isTheGamePaused || initialPanel.activeSelf || informationPanel.activeSelf) return;
        Time.timeScale = timeScale;
    }

    public void StartMeasurement()
    {
        playerMovementInput.enabled = auxiliarCamera1.enabled = auxiliarCamera2.enabled = true;
        auxiliarCamera1.LookAt = auxiliarCamera1.Follow = auxiliarCamera2.LookAt = auxiliarCamera2.Follow = player.transform;

        initialPanel.SetActive(false);
        speedometerContainer.SetActive(true);
        speedMeasurer.StartMeasurement(player, UpdateSpeedometer);
    }

    private void UpdateSpeedometer(MeasurePoint measurePoint)
    {
        speedometer.text = $"{Math.Round(measurePoint.AverageSpeed, 2)} m/s";
        UpdateSpeedLimitIndicator(measurePoint.AverageSpeed);
        if (playerClone.activeSelf) UpdatePlayerCloneAnimation(measurePoint.AverageSpeed);
    }

    private void UpdateSpeedLimitIndicator(double averageSpeed)
    {
        int nextSliderValue = 1;

        if (averageSpeed < 0.5) nextSliderValue = 1;
        else if (averageSpeed >= 0.5 && averageSpeed < 1) nextSliderValue = 2;
        else if (averageSpeed >= 1.5 && averageSpeed < 2) nextSliderValue = 3;
        else if (averageSpeed >= 2.5 && averageSpeed < 3) nextSliderValue = 4;
        else if (averageSpeed >= 3) nextSliderValue = 5;

        if (this.nextSliderValue == nextSliderValue) return;
        else this.nextSliderValue = nextSliderValue;

        changeToUp = speedLimitIndicator.value < nextSliderValue;
        isSliderChanging = true;
    }

    private void UpdatePlayerCloneAnimation(double averageSpeed)
    {
        float value = 0;
        
        if (averageSpeed == 0) value = 0;
        else if (averageSpeed > 0 && averageSpeed < 1) value = 0.4f;
        else if (averageSpeed >= 1) value = 1f;

        playerClone.GetComponent<Animator>().SetFloat("Blend", value, 0.2f, Time.deltaTime);
    }

    public void FinishMeasurement()
    {
        speedMeasurer.StopMeasurement();

        player.SetActive(false);
        playerMovementInput.enabled = auxiliarCamera1.enabled = auxiliarCamera2.enabled = false;
        StartCoroutine(GenerateResultWithDelay(1f));
    }

    private IEnumerator GenerateResultWithDelay(float delayInSeconds)
    {
        yield return new WaitForSeconds(delayInSeconds);
        informationPanel.SetActive(true);
        exampleResultView.GenerateResult(speedMeasurer.MeasurePoints);
    }

    public void StartReplayMode()
    {
        auxiliarCamera1.enabled = auxiliarCamera2.enabled = true;
        auxiliarCamera1.LookAt = auxiliarCamera1.Follow = auxiliarCamera2.LookAt = auxiliarCamera2.Follow = playerClone.transform;

        informationPanel.SetActive(false);
        replayController.StartReplayMode(playerClone, UpdateSpeedometer, FinishedReplay);
    }

    private void FinishedReplay()
    {
        auxiliarCamera1.enabled = auxiliarCamera2.enabled = false;
        informationPanel.SetActive(true);
        playerClone.SetActive(false);
        playerClone.transform.position = cloneOriginalPosition;
    }

    public void Restart() => SceneManager.LoadScene(SceneManager.GetActiveScene().name);

    public void QuitApplication() => Application.Quit();
}
