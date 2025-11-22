<p align="center">
  <img src="Docs/img/logo_no_bg.png" alt="RetroVR Logo" width="200"/>
</p>

# 🎮 RetroVR

RetroVR é um projeto de emulação de consoles retrô desenvolvido em **Unity** com foco em **Realidade Virtual (Meta Quest)**.
O objetivo é criar uma experiência imersiva onde o jogador interage fisicamente com consoles, cartuchos e TVs dentro de um ambiente 3D.

O projeto é **standalone**, não precisa de PC para rodar, e é focado em uma experiência nostálgica e intuitiva.

---

## 🌎 Read this in English
➡️ [README_EN.md](./README_EN.md)

---

## ✨ Principais Funcionalidades

- Emulação de consoles retrô via **Libretro**.
- Ambientes 3D em VR com interação física.
- Consoles funcionais com **slot de cartucho físico**.
- Cartuchos interativos com sistema de “inserir e executar”.
- TV funcional em VR com:
  - vídeo em tempo real
  - áudio funcional

---

## ✅ Estado Atual do Projeto

Atualmente o projeto já possui:

✅ Sala VR funcional (placeholder)

✅ Criação de pastas externas automáticas

✅ Console físico funcional em VR
- Slot real de cartucho
- Ligamento automático ao inserir cartucho

✅ Cartuchos físicos interativos (XRGrab)

✅ TV física funcional em VR
- Saída de vídeo já funcional
- Áudio já funcional

✅ Sistema de Prefabs para:
- Consoles
- Cartuchos

✅ Configuração por script:
- Core por console
- Extensões aceitas
- Override de core por cartucho

O fluxo completo já funciona:

**Pegar cartucho → Inserir → Console liga → Jogo roda com vídeo e som**

---

## 🛠️ Tecnologias / Libs

| Tecnologia        | Versão / Badge                                                                 |
|-------------------|------------------------------------------------------------------------------|
| Unity             | ![-Unity 6000.2.12f1](https://img.shields.io/badge/Unity-6000.1.14f1-blue.svg) |
| XR Interaction Toolkit | ![XR Interaction Toolkit](https://img.shields.io/badge/XR%20Interaction%20Toolkit-3.1.2-blue.svg) |
| OpenXR Plugin     | ![OpenXR Plugin](https://img.shields.io/badge/Open%20XR-1.15.1-blue.svg) |
| Newtonsoft Json   | ![JSON](https://img.shields.io/badge/Newtonsofg%20Json-3.2.1-green.svg)                    |
| [SK.Libretro](https://github.com/Skurdt/SK.Libretro)       | ![Libretro](https://img.shields.io/badge/Libretro-0.9.2-green.svg)           |
| C#                | ![C#](https://img.shields.io/badge/C%23-gray.svg?logo=c-sharp&logoColor=white) |

> ⚠️ **Aviso importante**
> O projeto foi atualizado da Unity **6000.1.14f1** para **6000.2.12f1** devido a vulnerabilidades de segurança identificadas na versão anterior.
> Recomendamos fortemente que contribuidores utilizem apenas a versão atual para evitar problemas de compatibilidade e riscos de segurança.


---

## 📂 Estrutura de Pastas Externas

```plaintext
com.unity.RetroVR/
├── files
│   ├── Libretro/
│   │   ├── config/
│   │   ├── roms/
│   │   ├── cores/
│   │   ├── labels/
│   │   ├── saves/
│   │   └── worldSaves/
```

---

## 🚀 Como Instalar

1. Baixe o APK na seção de **Releases**.
2. Instale no Meta Quest usando o **SideQuest**.

---

## 🗺️ Roadmap

O roadmap completo foi movido para arquivos separados:

- 🇧🇷 Português: [ROADMAP.md](./ROADMAP.md)
- 🇺🇸 English: [ROADMAP_EN.md](./ROADMAP_EN.md)

---

## 🤲 Como Contribuir

O guia de contribuição também está separado:

- 🇧🇷 Português: [CONTRIBUTING.md](./CONTRIBUTING.md)
- 🇺🇸 English: [CONTRIBUTING_EN.md](./CONTRIBUTING_EN.md)

---

## 📸 Screenshots

<p align="center">
  <img src="Docs/img/first_print.png" alt="First Print" width="600"/>
</p>

---

## 📄 Licença

Este projeto está sob a licença **MIT**.
Veja o arquivo [LICENSE](./LICENSE).

---

## 💬 Contato

Abra uma issue em:
https://github.com/SophiaFrassetto/RetroVR/issues
