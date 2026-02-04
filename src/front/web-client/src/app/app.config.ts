import { registerLocaleData } from '@angular/common';
import { provideHttpClient, withInterceptors } from '@angular/common/http';
import localeFr from '@angular/common/locales/fr';
import { ApplicationConfig, LOCALE_ID, provideBrowserGlobalErrorListeners } from '@angular/core';
import { provideRouter, withComponentInputBinding } from '@angular/router';
import { provideToastr } from 'ngx-toastr';
import { routes } from './app.routes';
import { provideTranslations } from './config/provideTranslations';
import { LangInterceptor } from './services/api/interceptors/lang-interceptor';
import { APP_BASE_URL } from './services/nswag/api-nswag-client';
import { ConfigService } from './services/config.service';
registerLocaleData(localeFr);

export const appConfig: ApplicationConfig = {
  providers: [
    provideBrowserGlobalErrorListeners(),
    provideHttpClient(withInterceptors([LangInterceptor])),
    provideRouter(routes, withComponentInputBinding()),
    {
      provide: APP_BASE_URL,
      useFactory: (configService: ConfigService) => configService.getConfig().apiUrl,
      deps: [ConfigService]
    },
    provideTranslations(),
    provideToastr(),
    { provide: LOCALE_ID, useValue: 'fr-FR' },
  ],
};
