import { BrowserModule } from '@angular/platform-browser';
import { NgModule, ErrorHandler, APP_INITIALIZER } from '@angular/core';

import { AppComponent } from './components/app/app.component';
import { HttpClientModule } from '@angular/common/http';
import { AppErrorHandler } from './common/exceptionHandling/app-error-handler';
import { ScheduleComponent } from './components/schedule/schedule.component';
import { ScheduleService } from './common/services/schedule.service';

@NgModule({
  declarations: [
    AppComponent,
    ScheduleComponent
     
  ],
  imports: [
    BrowserModule,
    HttpClientModule
  ],
  providers: [ScheduleService,
    { provide: ErrorHandler, useClass: AppErrorHandler }],
  bootstrap: [AppComponent]
})
export class AppModule { }
