# Práctica 6 - XR Interaction Toolkit

Asignatura: Interfaces Inteligentes  
Universidad: Universidad de La Laguna  
Titulación: Grado en Ingeniería Informática  
Curso: 2026/2027  
Fecha: 23 de octubre de 2026

Proyecto de Unity con el **XR Interaction Toolkit** en el que el jugador interactúa con tres objetos colocados sobre un plano a distintas distancias:

- Un **cubo** y una **esfera**, que se agarran con **Ray Interactor** (desde lejos) y **cambian de color** al seleccionarlos.
- Un **objeto de la Asset Store** (una taza), que se agarra con **Direct Interactor** (por contacto) y **actualiza un texto de la UI** al agarrarlo y soltarlo.

![](./Gifs/Gif.gif)

## 1. Entorno

| Elemento | Versión / detalle |
|---|---|
| Unity | 6.6 (6000.6.3f1) |
| Sistema | macOS (chip Intel) |
| Paquetes XR | XR Plugin Management, OpenXR Plugin, XR Interaction Toolkit (con Starter Assets importados) |
| Simulación | **XR Device Simulator** del XR Interaction Toolkit |

> Se usa el simulador del propio XR Interaction Toolkit y no el Meta XR Simulator, ya que este último no está disponible en macOS.

![](./Screenshots/Screenshot_1.jpeg)

## 2. Configuración inicial

1. Instalación de **XR Plugin Management**, **OpenXR Plugin** y **XR Interaction Toolkit**.
2. Importación de **Samples → Starter Assets** para disponer de prefabs ya configurados (XR Origin, mandos, interactores).
3. Activación de **OpenXR** en *XR Plugin Management* (pestaña Standalone) y marcado de los perfiles de interacción (Oculus Touch Controller Profile, etc.).
4. Uso del **XR Device Simulator** para probar en Play Mode sin gafas.

![](./Screenshots/Screenshot_2.jpeg)

## 3. Estructura de la escena

- **Eliminada la Main Camera**: el XR Origin trae su propia cámara y dos cámaras en escena pueden provocar problemas de vista.
- **XR Origin (XR Rig)** de los Starter Assets: define el suelo, la cámara (cabeza) y los dos mandos con sus interactores.
- **XR Interaction Manager** y **XR Device Simulator** en la escena.
- **Plano** como suelo.
- **Cubo**, **esfera** y **taza** colocados a diferentes distancias del jugador.
- **Canvas (World Space)** con un texto (`StatusText`) que muestra el estado de la taza.

![](./Screenshots/Screenshot_3.jpeg)

## 4. Objetos interactuables

Cada objeto necesita tres componentes:

| Componente | Qué aporta | Si falta |
|---|---|---|
| Collider | Permite que los interactores lo detecten | No es detectado |
| Rigidbody | Física: cae, rebota, se puede lanzar | No responde a la física al soltarlo |
| XR Grab Interactable | Lo marca como agarrable y define su comportamiento | No se puede agarrar |

### Cubo y esfera (Ray Interactor)

- Posiciones aproximadas: cubo en `(-0.5, 0.5, 2)` y esfera en `(0.5, 0.5, 4)`, es decir, a 2 m y 4 m del jugador.
- Se agarran apuntando con el rayo del mando y pulsando Select.
- Al seleccionarlos cambian de color (rojo en el cubo, azul en la esfera, configurable desde el Inspector) y al soltarlos recuperan el color original.

### Taza de la Asset Store (Direct Interactor)

- Asset utilizado: **[NOMBRE DEL ASSET Y ENLACE A LA ASSET STORE]**
- Colocada cerca del jugador, ya que el agarre directo solo funciona por contacto.
- Se agarra acercando el mando a la taza y pulsando Select.
- Al agarrarla, el texto de la UI cambia a *"Objeto agarrado"* y al soltarla a *"Objeto soltado"*.

## 5. Scripts

Los scripts son intencionadamente sencillos y se basan en los eventos `selectEntered` y `selectExited` del `XRGrabInteractable`.

### `ColorOnSelect.cs` (cubo y esfera)

Guarda el color original del material, lo cambia al color elegido cuando el objeto es seleccionado y lo restaura al soltarlo.

```csharp
using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit.Interactables;

public class ColorOnSelect : MonoBehaviour
{
    public Color selectedColor = Color.red;

    Renderer rend;
    Color originalColor;
    XRGrabInteractable interactable;

    void Awake()
    {
        rend = GetComponent<Renderer>();
        originalColor = rend.material.color;
        interactable = GetComponent<XRGrabInteractable>();
    }

    void OnEnable()
    {
        interactable.selectEntered.AddListener(OnSelect);
        interactable.selectExited.AddListener(OnDeselect);
    }

    void OnDisable()
    {
        interactable.selectEntered.RemoveListener(OnSelect);
        interactable.selectExited.RemoveListener(OnDeselect);
    }

    void OnSelect(UnityEngine.XR.Interaction.Toolkit.SelectEnterEventArgs args)
    {
        rend.material.color = selectedColor;
    }

    void OnDeselect(UnityEngine.XR.Interaction.Toolkit.SelectExitEventArgs args)
    {
        rend.material.color = originalColor;
    }
}
```

### `UpdateUIOnGrab.cs` (taza)

Actualiza el texto de la UI cuando la taza se agarra o se suelta.

```csharp
using UnityEngine;
using TMPro;
using UnityEngine.XR.Interaction.Toolkit.Interactables;

public class UpdateUIOnGrab : MonoBehaviour
{
    public TMP_Text statusText;

    XRGrabInteractable interactable;

    void Awake()
    {
        interactable = GetComponent<XRGrabInteractable>();
    }

    void OnEnable()
    {
        interactable.selectEntered.AddListener(OnGrab);
        interactable.selectExited.AddListener(OnRelease);
    }

    void OnDisable()
    {
        interactable.selectEntered.RemoveListener(OnGrab);
        interactable.selectExited.RemoveListener(OnRelease);
    }

    void OnGrab(UnityEngine.XR.Interaction.Toolkit.SelectEnterEventArgs args)
    {
        statusText.text = "Objeto agarrado";
    }

    void OnRelease(UnityEngine.XR.Interaction.Toolkit.SelectExitEventArgs args)
    {
        statusText.text = "Objeto soltado";
    }
}
```

> ✏️ **[Si usaste otros scripts o los modificaste, actualiza esta sección con el código final.]**

---

## 6. UI

- **Canvas** en modo *World Space*, escala `(0.001, 0.001, 0.001)`, tamaño `(400, 200)`, situado delante del jugador.
- Un **TextMeshPro** llamado `StatusText` con el texto inicial *"Suelta"*, referenciado desde el script `UpdateUIOnGrab` de la taza.

> 📷 **[CAPTURA 10: Canvas y StatusText en el Inspector / en la escena]**

---

## 7. Cómo probarlo

1. Abrir la escena del proyecto en Unity y pulsar **Play**.
2. Usar el **XR Device Simulator**: el propio simulador muestra en pantalla los controles para mover el rig, la cámara y cada mando.
3. Apuntar con el rayo al **cubo** y a la **esfera** y agarrar: cambian de color, y al soltar vuelven al original.
4. Acercar el mando a la **taza** y agarrar: el texto de la UI cambia a *"Objeto agarrado"*; al soltar, a *"Objeto soltado"*.

---

## 8. Problemas encontrados y soluciones

- **No existía el componente `XR Direct Interactor`** en esta versión del toolkit, donde el *Near-Far Interactor* agrupa el agarre cercano y el lejano. Se tuvo que buscar otra forma de conseguir el agarre directo de la taza. *(Ver sección 4 para la solución final.)*
- **`NullReferenceException` en el script** al soltar un objeto: se debía a una referencia sin asignar en el Inspector. Se resolvió comprobando que todos los campos públicos del script estaban asignados.
- **Namespaces distintos entre versiones del toolkit**: en XRI 3.x los interactables están en `UnityEngine.XR.Interaction.Toolkit.Interactables`.

> ✏️ **[Añade o corrige aquí cualquier otro problema que encontraras.]**

---

## 9. Recursos

- Documentación del XR Interaction Toolkit: https://docs.unity3d.com/Packages/com.unity.xr.interaction.toolkit@latest
- Asset de la Asset Store: **[ENLACE]**
