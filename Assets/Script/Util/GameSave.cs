using UnityEngine;

public class GameSave 
{
   #region RESOLUTION
   const string CONFIG_RESOLUTION = "RESOLUTION";

   public static void SaveScreenResolution(int intResolution)
   {
      Debug.Log($"Saving Screen Resolution {intResolution}");
      PlayerPrefs.SetInt(CONFIG_RESOLUTION, intResolution);
   }

   public static int LoadScreenResolution(int defaultResolution)
   {
      int savedIndex = PlayerPrefs.GetInt(CONFIG_RESOLUTION, defaultResolution);
      Debug.Log($"Loading Screen Resolution {savedIndex}");
      return savedIndex;
   }
   #endregion
   
   #region FULLSCREEN
   const string CONFIG_FULLSCREEN = "FULLSCREEN";
   
   public static void SaveFullScreenResolution(bool isFullScreen)
   {
      PlayerPrefsX.SetBool(CONFIG_FULLSCREEN, isFullScreen);
   }

   public static bool LoadFullScreenResolution(bool defaultFullScreen)
   {
      return PlayerPrefsX.GetBool(CONFIG_FULLSCREEN, defaultFullScreen);
   }
   
   #endregion
   
   #region VOLUME

   public static void SaveVolume(string volumeName, float currentVolume)
   {
      PlayerPrefs.SetFloat(volumeName, currentVolume);
   }

   public static float LoadVolume(string volumeName, float defaultVolume)
   {
      return PlayerPrefs.GetFloat(volumeName, defaultVolume);
   }
   
   #endregion
}
