/* Author : QFord Lin
 * Date : 2023-10-31
 * Unity Version : 2023.1.17f1c1
 * Descption : AssetPostprocessor Sample FROM P13
 */

using UnityEditor;

public class MyAsset : AssetPostprocessor
{ 

    /// <summary>
    /// Auto Generate by Copilot
    /// </summary>
    void OnPreprocessTexture()
    {
        TextureImporter textureImporter = (TextureImporter)assetImporter;
        textureImporter.textureType = TextureImporterType.Sprite;
        textureImporter.spritePixelsPerUnit = 100;
        textureImporter.mipmapEnabled = false;
    }

    private void OnPostprocessTexture(UnityEngine.Texture2D texture)
    {
        context.LogImportError("AssetPostprocessor:����");
        //��������ִ�к���жϣ��������䲻�����
        context.LogImportWarning("AssetPostprocessor:����");
    }
}


