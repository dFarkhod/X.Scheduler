import { Component, OnInit } from '@angular/core';
import { ScheduleService } from '../../common/services/schedule.service';
import { Schedule } from '../../dto/schedule';


@Component({
  selector: 'app-schedule',
  templateUrl: './schedule.component.html',
  styleUrls: ['./schedule.component.css']
})
export class ScheduleComponent implements OnInit {

  schedules: Schedule[];
  columnNames: string[];

  constructor(private scService: ScheduleService) { }

  ngOnInit() {
    this.scService.getAll<Schedule>()
      .subscribe(
        schedules => {
          this.schedules = schedules;
          this.columnNames = this.schedules[0].columns.split(',');
        })

  }

}
