import { NgModule, ModuleWithProviders } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule, ReactiveFormsModule, FormBuilder } from '@angular/forms';
import { RouterModule } from '@angular/router';
import { HttpModule, JsonpModule } from '@angular/http';
import { NgbModule } from '@ng-bootstrap/ng-bootstrap';

// Services
import { DataService } from './services/data.service';
import { BasketWrapperService} from './services/basket.wrapper.service';
import { SecurityService } from './services/security.service';
import { ConfigurationService } from './services/configuration.service';
import { StorageService } from './services/storage.service';

// Components:
import { Pager } from './components/pager/pager';
import { Header } from './components/header/header';
import { Identity } from './components/identity/identity';

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
        Pager,
        Header,
        Identity
    ],
    exports: [
        // Modules
        CommonModule,
        FormsModule,
        ReactiveFormsModule,
        RouterModule,
        NgbModule,
        // Providers, Components, directive, pipes
        Pager,
        Header,
        Identity
    ]
})
export class SharedModule {
    static forRoot(): ModuleWithProviders {
        return {
            ngModule: SharedModule,
            providers: [
                // Providers
                DataService,
                BasketWrapperService,
                SecurityService, 
                ConfigurationService, 
                StorageService
            ]
        };
    }
}
