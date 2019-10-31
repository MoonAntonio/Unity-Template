# Unity Project Template
[Github template repository](https://help.github.com/en/articles/creating-a-template-repository) for [Unity](https://unity.com/)

Una pequeña herramienta (**Unity Template -> Setup Unity para Git**) que se puede usar para limpiar rápidamente el repositorio si se usan diferentes versiones de Unity.

---

# ¿Qué hace?

Comprueba que el editor está configurado para forzar texto y meta archivos visibles, también puede eliminar cualquiera de los paquetes agregados por el proceso de actualización de Unity que no desea(**algunos de estos se agregan para ayudarlo**), luego fuerza una nueva serialización de todos los meta archivos. La herramienta se puede utilizar para eliminarse.

---

# TEMP

La carpeta temp, contiene dos archivos:

* .gitattributes: Contiene archivos para el final de linea de unity y la configuracion lfs de git.
* .editorconfig: Contiene una configuracion para el editor de unity y el IDE.

---

Si se quieren usar, solo ponerlos en el archivo raiz del proyecto.

---


# Testeado en
 - 2019.1
 - 2018.4