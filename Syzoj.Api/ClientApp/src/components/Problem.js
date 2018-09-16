import React, { Component } from 'react';

export class Problem extends Component {
  displayName = Problem.name

  constructor(props) {
    super(props);
    this.state = { statement: null, loading: true };
  }

  componentDidMount()
  {
    fetch('api/problemset/1/problem/' + this.props.match.params.id)
      .then(response => response.json())
      .then(data => {
        this.setState({ statement: data, loading: false });
      });
  }

  render() {
    return this.state.loading ?
        <p>Loading...</p>
    :
    <div>
        <h1>{this.state.statement.Title}</h1>
        <p>{JSON.stringify(this.state.statement)}</p>
    </div>;
  }
}
