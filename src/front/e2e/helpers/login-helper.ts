import { expect, Page } from 'playwright/test';
import { setDefaultLanguage } from './language-helper';
import { setFixedTime } from './time-helper';

export async function login(page: Page, userName: 'admin' | 'user', expectedHomeComponent?: string) {
  setFixedTime(page);
  setDefaultLanguage(page);

  await page.goto(getBaseUrl() + '/login');
  await expect(page.getByLabel('Identifiant')).toBeVisible({ timeout: 10000 });
  await page.getByLabel('Identifiant').fill(userName);
  await page.getByLabel('Mot de passe').fill('Secret01');
  await page.getByRole('button', { name: 'Se connecter' }).click();
  await expect(page.locator(expectedHomeComponent ?? 'app-home')).toBeVisible();
}

export async function logout(page: Page) {
  await page.goto('/');
  await page.locator('app-user-dropdown').click();
  await page.getByRole('button', { name: 'Déconnexion' }).click();
}

export function getBaseUrl() {
  return process.env.BASE_URL ?? 'http://localhost:44082';
}
