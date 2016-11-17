import { NgModule, ModuleWithProviders } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule, ReactiveFormsModule, FormBuilder } from '@angular/forms';
import { RouterModule } from '@angular/router';
import { HttpModule, JsonpModule } from '@angular/http';
import { NgbModule } from '@ng-bootstrap/ng-bootstrap';
import { TranslateModule, TranslateLoader } from 'ng2-translate/ng2-translate';

import { PageHeadingComponent } from './directives/page-heading.directive';
import { DynamicFormComponent } from './forms/dynamic-form.component';
import { DynamicFormControlComponent } from './forms/dynamic-form-control.component';
import { ErrorMessageComponent } from './forms/error-message.component';
import { ErrorSummaryComponent } from './forms/error-summary.component';
import { FormControlService } from './forms/form-control.service';

import { HeaderComponent } from './layout/header.component';
import { FooterComponent } from './layout/footer.component';
// Services
import { DataService } from './services/data.service';
import { ApiGatewayService } from './services/api-gateway.service';
import { AuthService } from './services/auth.service';
import { HttpErrorHandlerService } from './services/http-error-handler.service';
import { ApiTranslationLoader } from './services/api-translation-loader.service';
import { ContentService } from './services/content.service';
import { UtilityService } from './services/utility.service';
import { UppercasePipe } from './pipes/uppercase.pipe';

@NgModule({
  imports: [
    CommonModule,
    FormsModule,
    ReactiveFormsModule,
    RouterModule,
    NgbModule.forRoot(),
    // No need to export as these modules don't expose any components/directive etc'
    HttpModule,
    JsonpModule,
    TranslateModule.forRoot({ provide: TranslateLoader, useClass: ApiTranslationLoader })
  ],
  declarations: [
    DynamicFormComponent,
    DynamicFormControlComponent,
    ErrorMessageComponent,
    ErrorSummaryComponent,
    FooterComponent,
    HeaderComponent,
    PageHeadingComponent,
    UppercasePipe
  ],
  exports: [
    // Modules
    CommonModule,
    FormsModule,
    ReactiveFormsModule,
    RouterModule,
    NgbModule,
    TranslateModule,
    // Providers, Components, directive, pipes
    DynamicFormComponent,
    DynamicFormControlComponent,
    ErrorSummaryComponent,
    ErrorMessageComponent,
    FooterComponent,
    HeaderComponent,
    PageHeadingComponent,
    UppercasePipe
  ]

})
export class SharedModule {
  static forRoot(): ModuleWithProviders {
    return {
      ngModule: SharedModule,
      providers: [
        // Providers
        HttpErrorHandlerService,
        ApiGatewayService,
        AuthService,
        DataService,
        ContentService,
        FormControlService,
        UtilityService
      ]
    };
  }
}
