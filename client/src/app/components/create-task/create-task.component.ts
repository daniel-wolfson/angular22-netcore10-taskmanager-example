import { Component, inject, output } from '@angular/core';
import { FormBuilder, ReactiveFormsModule, Validators } from '@angular/forms';
import { TaskItem } from '../../models/task-item.model';
import { CreateTaskRequest, TaskService } from '../../services/task.service';

@Component({
  selector: 'app-create-task',
  standalone: true,
  imports: [ReactiveFormsModule],
  templateUrl: './create-task.component.html',
  styleUrl: './create-task.component.css'
})
export class CreateTaskComponent {
  private readonly formBuilder = inject(FormBuilder);
  private readonly taskService = inject(TaskService);

  readonly created = output<TaskItem>();

  readonly form = this.formBuilder.group({
    title: ['', [Validators.required, Validators.minLength(3), Validators.maxLength(100)]],
    description: ['']
  });

  submit(): void {
    if (this.form.invalid) {
      this.form.markAllAsTouched();
      return;
    }

    const request = this.form.getRawValue() as CreateTaskRequest;
    this.taskService.createTask(request).subscribe((task) => {
      this.created.emit(task);
      this.form.reset({ title: '', description: '' });
      this.form.markAsPristine();
      this.form.markAsUntouched();
    });
  }
}
