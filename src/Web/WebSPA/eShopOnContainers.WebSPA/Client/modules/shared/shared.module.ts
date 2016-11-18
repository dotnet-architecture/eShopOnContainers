import { NgModule, ModuleWithProviders } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule, ReactiveFormsModule, FormBuilder } from '@angular/forms';
import { RouterModule } from '@angular/router';
import { HttpModule, JsonpModule } from '@angular/http';
import { NgbModule } from '@ng-bootstrap/ng-bootstrap';

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
import { UtilityService } from './services/utility.service';
import { UppercasePipe } from './pipes/uppercase.pipe';

//Components:
import {Pager } from './components/pager/pager';

@NgModule({
  imports: [
    CommonModule,
    FormsModule,
    ReactiveFormsModule,
    RouterModule,
    NgbModule.forRoot(),
    // No need to export as these modules don't expose any components/directive etc'
    HttpModule,
    JsonpModule
  ],
  declarations: [
    DynamicFormComponent,
    DynamicFormControlComponent,
    ErrorMessageComponent,
    ErrorSummaryComponent,
    FooterComponent,
    HeaderComponent,
    PageHeadingComponent,
    UppercasePipe, 
    Pager
  ],
  exports: [
    // Modules
    CommonModule,
    FormsModule,
    ReactiveFormsModule,
    RouterModule,
    NgbModule,
    // Providers, Components, directive, pipes
    DynamicFormComponent,
    DynamicFormControlComponent,
    ErrorSummaryComponent,
    ErrorMessageComponent,
    FooterComponent,
    HeaderComponent,
    PageHeadingComponent,
    UppercasePipe, 
    Pager
  ]

})
export class SharedModule {
  static forRoot(): ModuleWithProviders {
    return {
      ngModule: SharedModule,
      providers: [
        // Providers
        DataService,
        FormControlService,
        UtilityService
      ]
    };
  }
}
