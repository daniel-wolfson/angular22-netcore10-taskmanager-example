import { DatePipe } from '@angular/common';
import { Component, computed, input, output, signal } from '@angular/core';
import { TaskItem } from '../../models/task-item.model';

@Component({
  selector: 'app-task-list',
  standalone: true,
  imports: [DatePipe],
  templateUrl: './task-list.component.html',
  styleUrl: './task-list.component.css'
})
export class TaskListComponent {
  readonly tasks = input.required<TaskItem[]>();
  readonly markDone = output<string>();

  readonly search = signal('');

  readonly filteredTasks = computed(() => {
    const term = this.search().trim().toLowerCase();

    return this.tasks()
      .filter((task) => task.title.toLowerCase().includes(term))
      .sort((a, b) => new Date(b.createdDate).getTime() - new Date(a.createdDate).getTime());
  });

  setSearch(value: string): void {
    this.search.set(value);
  }

  markAsDone(id: string): void {
    this.markDone.emit(id);
  }
}
