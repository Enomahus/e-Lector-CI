import { setTestLanguage } from '@helpers/language-helper';
import { setTestTime } from '@helpers/time-helper';
import test, { expect } from 'playwright/test';

test.describe('Connexion', () => {
  setTestTime(test);
  setTestLanguage(test);

  test('La connexion fonctionne', async ({ page }) => {
    await page.goto('/login');
    await page.getByLabel('Identifiant').fill('user1');
    await page.getByLabel('Mot de passe').fill('Secret01');
    await page.getByRole('button', { name: 'Se connecter' }).click();

    await expect(page.locator('app-home')).toBeInViewport();
  });

  test('Validation du formulaire de connexion', async ({ page }) => {
    await page.goto('/login');
    const loginBtn = page.getByRole('button', { name: 'Se connecter' });
    await expect(loginBtn).toBeDisabled();

    await page.getByLabel('Identifiant').fill('user1');
    await expect(loginBtn).toBeDisabled();

    await page.getByLabel('Mot de passe').fill('Secret01');
    await expect(loginBtn).toBeEnabled();

    await page.getByLabel('Identifiant').fill('');
    await expect(loginBtn).toBeDisabled();
  });
});
