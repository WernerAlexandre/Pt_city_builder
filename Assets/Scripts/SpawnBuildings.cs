using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using UnityEngine.UI;

public class SpawnBuildings : MonoBehaviour
{
    #region Inspector Variables
    [TooltipAttribute("The tile GameObject that make up the grid")]
    [SerializeField] GameObject productionTile;

    [TooltipAttribute("The layer in which the terrain is placed")]
    [SerializeField] LayerMask terrainLayer;

    [TooltipAttribute("Need GraphicRaycaster to detect click on a button")]
    [SerializeField] GraphicRaycaster uiRaycaster;

    [SerializeField] GameObject underConstructionGO;
    [SerializeField] BuildProgressSO buildingToPlace;
    #endregion

    #region Instance Objects
    GameObject currentSpawnedBuilding;
    RaycastHit hit;
    List<ProductionTile> activeTiles;
    GameObject activeTilesParent;
    #endregion

    Vector3 truePos;
    public float gridSize;

    void Start ()
    {
        activeTiles = new List<ProductionTile>();
        if (!productionTile)
            Debug.LogError("Production Tile is NULL");
        if (!uiRaycaster)
            Debug.Log("GraphicRaycaster not found! Will place objects on button click");
	}


    void Update()
    {
        if (currentSpawnedBuilding)
        {
            // Si clic gauche
            if (Input.GetMouseButtonDown(0))
            {
                if (!PlacementHelpers.RaycastFromMouse(out hit, terrainLayer))
                    return;
                float gridDecal = 0.5f;
                // Déplacer la grille de construction suivant l'axe X 
                truePos.x = Mathf.Ceil(hit.point.x / gridSize) * gridSize - gridDecal;
                // Garder la grille de construction constante sur l'axe Y
                truePos.y = currentSpawnedBuilding.gameObject.transform.localScale.y/2;
                // Déplacer la grille de construction suivant l'axe Z
                truePos.z = Mathf.Ceil(hit.point.z / gridSize) * gridSize - gridDecal;

                // Donne la position réel de l'objet à construire par rapport à la souris
                currentSpawnedBuilding.transform.position = truePos;

                // Si il n'y a pas de collision lancer la construction de l'objet
                if(CanPlaceBuilding())
                    PlaceBuilding();
            }
            // Si clic droit annuler la prévisualisation de la zone de construction
            if (Input.GetMouseButtonDown(1)){
                Destroy(currentSpawnedBuilding);
                ClearGrid();
            }
        }
    }


    void FixedUpdate()
    {
        if(currentSpawnedBuilding)
            if(PlacementHelpers.RaycastFromMouse(out hit, terrainLayer)){
                // Positionne l'objet suivant l'axe X 
                truePos.x = Mathf.Ceil(hit.point.x / gridSize) * gridSize;
                // Garder l'objet constante sur l'axe Y
                truePos.y = 0;
                // Positionne l'objet suivant l'axe Z
                truePos.z = Mathf.Ceil(hit.point.z / gridSize) * gridSize;

                // Donne la position réel de l'objet à construire
                currentSpawnedBuilding.transform.position = truePos;
            }
    }


    bool CanPlaceBuilding()
    {
        if (PlacementHelpers.IsButtonPressed(uiRaycaster))
            return false;
        for(int i = 0; i < activeTiles.Count; i++)
            if(activeTiles[i].colliding)
                return false;
        return true;
    }


    void PlaceBuilding()
    {
        ClearGrid();
        StartCoroutine(BeginBuilding());
    }


    void ClearGrid()
    {
        Destroy(activeTilesParent);
        activeTiles.RemoveAll(i => i);
    }


    IEnumerator BeginBuilding()
    {
        Vector3 pos = truePos;
        GameObject instance = currentSpawnedBuilding;
        currentSpawnedBuilding = null;
        Debug.Log(""+ truePos);

        RaycastHit hitTerrain;
        if (PlacementHelpers.RaycastFromMouse(out hitTerrain, terrainLayer))
            pos = hitTerrain.point;

        GameObject go = Instantiate(underConstructionGO, truePos, Quaternion.identity);
        yield return new WaitForSeconds(buildingToPlace.currentBuilding.buildTime);
        Debug.Log("waited " + buildingToPlace.currentBuilding.buildTime + " seconds to build " + buildingToPlace.currentBuilding.name);
        PlacementHelpers.ToggleRenderers(instance, true);
        Destroy(go);
    }


    void FillRectWithTiles(Collider col)
    {
        if (activeTilesParent)
            return;

        Rect rect = PlacementHelpers.MakeRectOfCollider(col);
        // Récupère la taille de l'objet à construire suivant l'axe X
        float toX = col.gameObject.transform.localScale.x/2;
        // Récupère la taille de l'objet à construire suivant l'axe X
        float toZ = col.gameObject.transform.localScale.z/2;

        GameObject parent = new GameObject("PlacementGrid");
        parent.transform.SetParent(col.gameObject.transform.root);
        parent.transform.position = col.gameObject.transform.InverseTransformPoint(new Vector3(0, 0.5f, 0));

        // Création de la zone de construction (suivant les axes X et Z) avec l'affichage des collisions 
        // pour montrer à l'utilisateur si la construction est possible ou non
        for(float i = -toX; i < toX; i ++)
        {
            for(float j = -toZ; j < toZ; j ++)
            {
                GameObject tile = Instantiate(productionTile);
                tile.transform.SetParent(parent.transform);
                tile.transform.position = new Vector3(i, parent.transform.position.y, j);
                activeTiles.Add(tile.GetComponent<ProductionTile>());
            }
        }
        activeTilesParent = parent;
    }


    public void SpawnBuilding(BuildingSO building)
    {
        // Si l'objet n'a pas été placé alors return
        if (currentSpawnedBuilding)
            return;

        currentSpawnedBuilding = Instantiate(building.buildingPrefab);
        buildingToPlace.currentBuilding = building;
        PlacementHelpers.ToggleRenderers(currentSpawnedBuilding, false);
        Collider[] cols = currentSpawnedBuilding.GetComponentsInChildren<Collider>();
        if(cols.Length > 0)
            FillRectWithTiles(cols[0]);
        else
            Debug.LogError("Building has no colliders");
    }
}
