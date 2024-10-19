## Membres du groupe

- **Clément GRASSET**
- **Rayan BELKESSAM**

## Points traités

- Ajouter un mode multi-joueur (entre deux joueurs humains)
- Côté front : ajouter des images pour les bateaux plutôt que des lettres
- Permettre au joueur de placer ses bateaux à sa façon (en dessinant les bateaux sur la grille)
- Ajouter de la sécurité sur l’application pour sécuriser les échanges et ajouter une gestion d’utilisateurs
- Améliorer le comportement de l’IA (attaque randomisée par périmètre, niveau difficile uniquement)
- Rajouter un niveau de difficulté (taille de la grille, nombre de bateaux, intelligence IA)

## Note

Par souci de cohérence, nous avons décidé généraliser le traitement des jeux, qu'ils soient contre une IA ou contre un joueur humain.
Nous avons donc décidé de toujours utiliser SignalR pour la communication entre les joueurs, même si l'adversaire est une IA.

Par conséquent, l'application contient très peu de routes HTTP, mais il devrait y en avoir suffisamment pour montrer que nous savons les utiliser, ainsi que la validation des données.