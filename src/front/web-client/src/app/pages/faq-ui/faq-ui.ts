import { Component, signal } from '@angular/core';

interface FaqItem {
  question: string;
  answer: string;
}

@Component({
  selector: 'app-faq-ui',
  imports: [],
  templateUrl: './faq-ui.html',
  styleUrl: './faq-ui.scss',
})
export class FaqUi {
  // Déclarations des propriétés textuelles via des signaux immuables
  title = signal<string>('Foire aux questions');
  subtitle = signal<string>('Trouvez les réponses à vos questions les plus fréquentes');

  // Tableau contenant vos questions/réponses injectées
  questions = signal<FaqItem[]>([
    {
      question: 'Comment réinitialiser mon mot de passe ?',
      answer:
        "Pour réinitialiser votre mot de passe, cliquez sur le lien 'Mot de passe oublié' sur la page de connexion et suivez les instructions pour recevoir un e-mail de réinitialisation.",
    },
    {
      question: 'Comment contacter le support ?',
      answer: 'Vous pouvez contacter notre support en envoyant un e-mail à support@example.com.',
    },
    {
      question: 'Comment créer un compte ?',
      answer:
        "Pour créer un compte, cliquez sur le lien 'S'inscrire' sur la page de connexion et remplissez le formulaire d'inscription.",
    },
  ]);

  // Signal d'état qui stocke l'index de la question actuellement ouverte (null si tout est fermé)
  openedIndex = signal<number | null>(null);

  /**
   * Alterne l'affichage de l'accordéon.
   * Si la question cliquée est déjà ouverte, elle se referme. Sinon, elle s'ouvre.
   */
  toggleQuestion(index: number): void {
    if (this.openedIndex() === index) {
      this.openedIndex.set(null);
    } else {
      this.openedIndex.set(index);
    }
  }
}
