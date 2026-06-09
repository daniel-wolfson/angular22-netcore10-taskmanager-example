import { Component, inject, signal } from '@angular/core';
import { CreateTaskComponent } from './components/create-task/create-task.component';
import { TaskListComponent } from './components/task-list/task-list.component';
import { TaskItem } from './models/task-item.model';
import { TaskService } from './services/task.service';

@Component({
  selector: 'app-root',
  standalone: true,
  imports: [CreateTaskComponent, TaskListComponent],
  templateUrl: './app.component.html',
  styleUrl: './app.component.css'
})
export class AppComponent {
  private readonly taskService = inject(TaskService);

  readonly tasks = signal<TaskItem[]>([]);

  constructor() {
    this.loadTasks();
  }

  loadTasks(): void {
    this.taskService.getTasks().subscribe((tasks) => {
      this.tasks.set(tasks);
    });
  }

  handleCreated(task: TaskItem): void {
    this.tasks.update((tasks) => [task, ...tasks]);
  }

  handleMarkDone(id: string): void {
    this.taskService.markAsDone(id).subscribe((updatedTask) => {
      this.tasks.update((tasks) =>
        tasks.map((task) => (task.id === updatedTask.id ? updatedTask : task))
      );
    });
  }
}
