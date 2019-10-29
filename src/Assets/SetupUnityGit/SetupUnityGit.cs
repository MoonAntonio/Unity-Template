//                                  ┌∩┐(◣_◢)┌∩┐
//                                                                              \\
// SetupUnityGit.cs (20/10/2015)                                                \\
// Autor: Antonio Mateo (.\Moon Antonio)    antoniomt.moon@gmail.com            \\
//******************************************************************************\\

#region Librerias
using UnityEngine;
using UnityEditor;
using UnityEditor.PackageManager;
using UnityEditor.PackageManager.Requests;
using System.Linq;
#endregion

namespace Moon.Internal
{
    /// <summary>
    /// <para>Setup de git para Unity3D.</para>
    /// </summary>
    public class SetupUnityGit : EditorWindow
    {
        #region Constantes
        /// <summary>
        /// <para>Titulo para los displays.</para>
        /// </summary>
        private const string TITULO = "Setup Unity: ";
        #endregion

        #region Variables Privadas
        /// <summary>
        /// <para>Determina si se configurara unity para git.</para>
        /// </summary>
        private bool isSetupGit = true;
        /// <summary>
        /// <para>Determina si se eliminaran paquetes por defecto.</para>
        /// </summary>
        private bool isEliminarPaquetes = true;
        /// <summary>
        /// <para>Determina si se eliminaran todos los paquetes por defecto.</para>
        /// </summary>
        private bool isEliminarPaquetesTodos = true;
        /// <summary>
        /// <para>Paquetes que se eliminaran.</para>
        /// </summary>
        private ToggleOpcion[] paquetesParaEliminar = new ToggleOpcion[] { };
        /// <summary>
        /// <para>Dimensiones del scroll para el apartado de paquetes.</para>
        /// </summary>
        private Vector2 scrollPaquetes;
        #endregion

        #region Estructuras
        /// <summary>
        /// <para>Opcion para mostrar los paquetes.</para>
        /// </summary>
        private struct ToggleOpcion
        {
            #region Variables
            /// <summary>
            /// <para>Determina si esta activo.</para>
            /// </summary>
            public bool isActivo;
            /// <summary>
            /// <para>Nombre de la opcion.</para>
            /// </summary>
            public readonly string nombre;
            #endregion

            #region Constructor
            /// <summary>
            /// <para>Constructor de <see cref="ToggleOpcion"/>.</para>
            /// </summary>
            /// <param name="isActivo">Determina si esta activo.</param>
            /// <param name="nombre">Nombre de la opcion.</param>
            public ToggleOpcion(bool isActivo, string nombre)
            {
                this.isActivo = isActivo;
                this.nombre = nombre;
            }
            #endregion
        }
        #endregion

        #region Menu
        /// <summary>
        /// <para>Menu de <see cref="SetupUnityGit"/>.</para>
        /// </summary>
        [MenuItem("Unity Template/Setup Unity para Git")]
        public static void SetupWindow()
        {
            SetupUnityGit window = GetWindow<SetupUnityGit>(true, "Setup Unity para Git", true);
            window.minSize = new Vector2(360, 360);
            window.maxSize = new Vector2(360, 1024);

            window.GenerarListaPaquetes();
        }
        #endregion

        #region GUI
        /// <summary>
        /// <para>GUI de <see cref="SetupUnityGit"/>.</para>
        /// </summary>
        private void OnGUI()
        {
            EditorGUILayout.BeginVertical(GUI.skin.box);
            MostrarAjustes(ref this.isSetupGit, "Setup editor para git", "Recuerda inicializar Git-LFS en el repositorio despues de la configuracion. \n\nNota: Si el archivo que deberia ser LFS ya esta empujado, no se convertira, hay herramientas para eso.");
            if (paquetesParaEliminar.Length > 0)
            {
                MostrarPaquetesEliminables();
            }
            EditorGUILayout.EndVertical();

            if (GUILayout.Button("Iniciar Setup"))
            {
                if (this.isSetupGit)
                {
                    EditorUtility.DisplayProgressBar(TITULO, "Setup Editor para Git", 0.2f);
                    SetEditorSettingsParaGit();
                    AssetDatabase.SaveAssets();
                }

                if (isEliminarPaquetes)
                {
                    EditorUtility.DisplayProgressBar(TITULO, "Eliminando paquetes", 0.4f);
                    if (EditorUtility.DisplayDialog("!!Advertencia!! Operacion destructiva", "Esto eliminara los paquetes seleccionados por completo", "¡¡Destruyelo!!"))
                    {
                        EliminarPaquetes();
                    }
                }

                AssetDatabase.ForceReserializeAssets();

                EditorUtility.ClearProgressBar();
            }
            if (GUILayout.Button("Eliminar SetupUnityParaGit"))
            {
                if (EditorUtility.DisplayDialog("!!Advertencia!! Operacion destructiva", "Esto eliminara SetupUnityParaGit por completo", "Aceptar", "Mantener"))
                {
                    Limpiar();
                    AssetDatabase.Refresh();
                }
            }
        }
        #endregion

        #region Metodos Privados
        /// <summary>
        /// <para>Genera la lista de paquetes del cliente.</para>
        /// </summary>
        protected void GenerarListaPaquetes()
        {
            ListRequest listaPaquetes = Client.List();
            while (!listaPaquetes.IsCompleted)
            {
            }

            this.paquetesParaEliminar = listaPaquetes.Result.Where(package => !package.name.Contains("package-manager-ui") && !package.name.Contains("com.unity.modules")).Select((package) => { return new ToggleOpcion(true, package.name); }).ToArray();

            if (paquetesParaEliminar.Length > 0)
            {
                this.isEliminarPaquetes = true;
            }
            else
            {
                this.isEliminarPaquetes = false;
            }
        }

        /// <summary>
        /// <para>Muestra los ajustes.</para>
        /// </summary>
        /// <param name="isConfigurado">Determina si esta configurado.</param>
        /// <param name="nombreAjustes">Nombre de los ajustes.</param>
        /// <param name="descripcion">Descripcion de los ajustes.</param>
        private void MostrarAjustes(ref bool isConfigurado, string nombreAjustes, string descripcion = "")
        {
            EditorGUILayout.BeginVertical(GUI.skin.box);
            isConfigurado = EditorGUILayout.Toggle(nombreAjustes, isConfigurado);
            if (isConfigurado && !string.IsNullOrEmpty(descripcion))
            {
                EditorGUI.indentLevel++;
                EditorGUILayout.LabelField(descripcion, EditorStyles.wordWrappedLabel);
                EditorGUI.indentLevel--;
            }
            EditorGUILayout.EndVertical();
        }

        /// <summary>
        /// <para>Cambia los ajustes del editor para git.</para>
        /// </summary>
        private void SetEditorSettingsParaGit()
        {
            EditorSettings.externalVersionControl = "Visible Meta Files";
            EditorSettings.serializationMode = SerializationMode.ForceText;

            Debug.LogWarning("Edit -> Project Settings -> Version Control: Cambiado a Visible Meta Files");
            Debug.LogWarning("Edit -> Project Settings -> Asset Serialization: Cambiado a Force Text");
        }

        /// <summary>
        /// <para>Muestra los paquetes eliminables.</para>
        /// </summary>
        private void MostrarPaquetesEliminables()
        {
            EditorGUILayout.BeginVertical(GUI.skin.box);

            this.isEliminarPaquetes = EditorGUILayout.Toggle("Eliminar Paquetes", isEliminarPaquetes);

            if (this.isEliminarPaquetes)
            {
                EditorGUI.indentLevel++;
                EditorGUILayout.LabelField("Elimina los paquetes seleccionados, agregados por defecto por Unity", EditorStyles.wordWrappedLabel);

                this.isEliminarPaquetesTodos = EditorGUILayout.Toggle("Todos", this.isEliminarPaquetesTodos);

                if (!this.isEliminarPaquetesTodos)
                {
                    this.scrollPaquetes = EditorGUILayout.BeginScrollView(this.scrollPaquetes);

                    for (int n = 0; n < paquetesParaEliminar.Length; n++)
                    {
                        paquetesParaEliminar[n].isActivo = EditorGUILayout.Toggle(paquetesParaEliminar[n].nombre, paquetesParaEliminar[n].isActivo);
                    }
                    EditorGUILayout.EndScrollView();
                }

                EditorGUI.indentLevel--;
            }
            EditorGUILayout.EndVertical();
        }

        /// <summary>
        /// <para>Elimina los paquetes seleccionados.</para>
        /// </summary>
        private void EliminarPaquetes()
        {
            RemoveRequest eliminarPaquete;

            foreach (var paquete in paquetesParaEliminar)
            {
                if (this.isEliminarPaquetesTodos || paquete.isActivo)
                {
                    eliminarPaquete = Client.Remove(paquete.nombre);
                    while (!eliminarPaquete.IsCompleted)
                    {
                    }
                }
            }
        }

        /// <summary>
        /// <para>Limpia todo.</para>
        /// </summary>
        private void Limpiar()
        {
            SetupUnityGit window = GetWindow<SetupUnityGit>(true, "Setup Unity Para Git", true);
            window.Close();
            Debug.LogWarning("Eliminando Unity Git Setup Tool");
            AssetDatabase.MoveAssetToTrash("Assets/SetupUnityGit");
        }
        #endregion
    }
}
